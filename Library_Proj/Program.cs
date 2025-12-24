using System;
using System.Linq;
using Library_Proj.Data;
using Library_Proj.Data.Repositories;
using Library_Proj.Models;
using Library_Proj.Services;

namespace Library_Proj
{
    class Program
    {
        private static LibraryContext _context;
        private static BookService _bookService;
        private static ReaderService _readerService;
        private static LoanService _loanService;
        private static ReservationService _reservationService;
        private static FineService _fineService;

        static void Main(string[] args)
        {
            InitializeServices();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== БИБЛИОТЕЧНАЯ ИНФОРМАЦИОННАЯ СИСТЕМА ===");

            // Создание базы данных при первом запуске
            bool created = _context.Database.EnsureCreated();
            if (created)
            {
                Console.WriteLine("База данных создана успешно!");
            }

            ShowMainMenu();
        }

        static void InitializeServices()
        {
            _context = new LibraryContext();

            // Инициализация репозиториев
            var bookRepo = new Repository<Book>(_context);
            var readerRepo = new Repository<Reader>(_context);
            var loanRepo = new Repository<Loan>(_context);
            var reservationRepo = new Repository<Reservations>(_context);
            var fineRepo = new Repository<Fine>(_context);

            // Инициализация сервисов
            _bookService = new BookService(bookRepo, reservationRepo);
            _readerService = new ReaderService(readerRepo);
            _fineService = new FineService(fineRepo, readerRepo);
            _reservationService = new ReservationService(reservationRepo, bookRepo);
            _loanService = new LoanService(loanRepo, bookRepo, readerRepo, reservationRepo, _fineService);
        }

        static void ShowMainMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
                Console.WriteLine("1. Управление книгами");
                Console.WriteLine("2. Управление читателями");
                Console.WriteLine("3. Выдача книг");
                Console.WriteLine("4. Возврат книг");
                Console.WriteLine("5. Резервирование книг");
                Console.WriteLine("6. Штрафы");
                Console.WriteLine("7. Поиск книг");
                Console.WriteLine("8. Просмотр активных выдач");
                Console.WriteLine("9. Просмотр активных резервов");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowBookMenu();
                        break;
                    case "2":
                        ShowReaderMenu();
                        break;
                    case "3":
                        GiveBook();
                        break;
                    case "4":
                        ReturnBook();
                        break;
                    case "5":
                        ReserveBook();
                        break;
                    case "6":
                        ShowFinesMenu();
                        break;
                    case "7":
                        SearchBooks();
                        break;
                    case "8":
                        ShowActiveLoans();
                        break;
                    case "9":
                        ShowActiveReservations();
                        break;
                    case "0":
                        Console.WriteLine("Выход из программы...");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }

        static void ShowBookMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== УПРАВЛЕНИЕ КНИГАМИ ===");
                Console.WriteLine("1. Просмотр всех книг");
                Console.WriteLine("2. Добавить книгу");
                Console.WriteLine("3. Редактировать книгу");
                Console.WriteLine("4. Удалить книгу");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllBooks();
                        break;
                    case "2":
                        AddBook();
                        break;
                    case "3":
                        EditBook();
                        break;
                    case "4":
                        DeleteBook();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }

        static void ShowAllBooks()
        {
            Console.WriteLine("\n=== СПИСОК ВСЕХ КНИГ ===");
            var books = _bookService.GetAll();

            if (!books.Any())
            {
                Console.WriteLine("Книги не найдены.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-30} {2,-25} {3,-15} {4,-5} {5,-8} {6,-8} {7,-15}",
                "ID", "Название", "Автор", "Жанр", "Год", "Всего", "Доступно", "Статус");
            Console.WriteLine(new string('-', 120));

            foreach (var book in books)
            {
                Console.WriteLine("{0,-5} {1,-30} {2,-25} {3,-15} {4,-5} {5,-8} {6,-8} {7,-15}",
                    book.Id,
                    Truncate(book.Title, 28),
                    Truncate(book.Author, 23),
                    Truncate(book.Genre, 13),
                    book.Year,
                    book.TotalCount,
                    book.AvailableCount,
                    book.Status);
            }
        }

        static void AddBook()
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОЙ КНИГИ ===");

            try
            {
                var book = new Book();

                Console.Write("Название: ");
                book.Title = Console.ReadLine();

                Console.Write("Автор: ");
                book.Author = Console.ReadLine();

                Console.Write("Жанр: ");
                book.Genre = Console.ReadLine();

                Console.Write("ISBN (не обязательно): ");
                book.ISBN = Console.ReadLine();

                Console.Write("Год издания: ");
                if (int.TryParse(Console.ReadLine(), out int year))
                    book.Year = year;
                else
                    throw new Exception("Неверный формат года");

                Console.Write("Общее количество экземпляров: ");
                if (int.TryParse(Console.ReadLine(), out int totalCount))
                    book.TotalCount = totalCount;
                else
                    throw new Exception("Неверный формат количества");

                _bookService.Add(book);
                Console.WriteLine("Книга успешно добавлена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void EditBook()
        {
            Console.WriteLine("\n=== РЕДАКТИРОВАНИЕ КНИГИ ===");

            try
            {
                Console.Write("Введите ID книги для редактирования: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Неверный формат ID");
                    return;
                }

                var book = _bookService.GetById(id);
                if (book == null)
                {
                    Console.WriteLine("Книга не найдена");
                    return;
                }

                Console.WriteLine($"Редактирование книги: {book.Title}");
                Console.WriteLine("(Оставьте поле пустым, чтобы не изменять)");

                Console.Write($"Название [{book.Title}]: ");
                string title = Console.ReadLine();
                if (!string.IsNullOrEmpty(title))
                    book.Title = title;

                Console.Write($"Автор [{book.Author}]: ");
                string author = Console.ReadLine();
                if (!string.IsNullOrEmpty(author))
                    book.Author = author;

                Console.Write($"Жанр [{book.Genre}]: ");
                string genre = Console.ReadLine();
                if (!string.IsNullOrEmpty(genre))
                    book.Genre = genre;

                Console.Write($"ISBN [{book.ISBN}]: ");
                string isbn = Console.ReadLine();
                if (isbn != "")
                    book.ISBN = isbn;

                Console.Write($"Год издания [{book.Year}]: ");
                string yearStr = Console.ReadLine();
                if (!string.IsNullOrEmpty(yearStr) && int.TryParse(yearStr, out int year))
                    book.Year = year;

                Console.Write($"Общее количество [{book.TotalCount}]: ");
                string totalStr = Console.ReadLine();
                if (!string.IsNullOrEmpty(totalStr) && int.TryParse(totalStr, out int totalCount))
                    book.TotalCount = totalCount;

                Console.Write($"Доступное количество [{book.AvailableCount}]: ");
                string availableStr = Console.ReadLine();
                if (!string.IsNullOrEmpty(availableStr) && int.TryParse(availableStr, out int availableCount))
                    book.AvailableCount = availableCount;

                _bookService.Update(book);
                Console.WriteLine("Книга успешно обновлена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void DeleteBook()
        {
            Console.WriteLine("\n=== УДАЛЕНИЕ КНИГИ ===");

            try
            {
                Console.Write("Введите ID книги для удаления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Неверный формат ID");
                    return;
                }

                Console.Write("Вы уверены? (да/нет): ");
                string confirm = Console.ReadLine();

                if (confirm?.ToLower() == "да")
                {
                    _bookService.Delete(id);
                    Console.WriteLine("Книга успешно удалена!");
                }
                else
                {
                    Console.WriteLine("Удаление отменено");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void ShowReaderMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== УПРАВЛЕНИЕ ЧИТАТЕЛЯМИ ===");
                Console.WriteLine("1. Просмотр всех читателей");
                Console.WriteLine("2. Добавить читателя");
                Console.WriteLine("3. Редактировать читателя");
                Console.WriteLine("4. Удалить читателя");
                Console.WriteLine("5. Поиск читателя");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllReaders();
                        break;
                    case "2":
                        AddReader();
                        break;
                    case "3":
                        EditReader();
                        break;
                    case "4":
                        DeleteReader();
                        break;
                    case "5":
                        SearchReader();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }

        static void ShowAllReaders()
        {
            Console.WriteLine("\n=== СПИСОК ВСЕХ ЧИТАТЕЛЕЙ ===");
            var readers = _readerService.GetAll();

            if (!readers.Any())
            {
                Console.WriteLine("Читатели не найдены.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-30} {2,-15} {3,-30} {4,-10}",
                "ID", "ФИО", "Билет", "Контакты", "Долг");
            Console.WriteLine(new string('-', 90));

            foreach (var reader in readers)
            {
                Console.WriteLine("{0,-5} {1,-30} {2,-15} {3,-30} {4,-10}",
                    reader.Id,
                    Truncate(reader.FullName, 28),
                    reader.TicketNumber,
                    Truncate(reader.ContactInfo ?? "нет", 28),
                    reader.HasDebt ? "Есть" : "Нет");
            }
        }

        static void AddReader()
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОГО ЧИТАТЕЛЯ ===");

            try
            {
                var reader = new Reader();

                Console.Write("ФИО: ");
                reader.FullName = Console.ReadLine();

                Console.Write("Номер читательского билета: ");
                reader.TicketNumber = Console.ReadLine();

                Console.Write("Контактная информация (не обязательно): ");
                reader.ContactInfo = Console.ReadLine();

                _readerService.Add(reader);
                Console.WriteLine("Читатель успешно добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void EditReader()
        {
            Console.WriteLine("\n=== РЕДАКТИРОВАНИЕ ЧИТАТЕЛЯ ===");

            try
            {
                Console.Write("Введите ID читателя для редактирования: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Неверный формат ID");
                    return;
                }

                var reader = _readerService.GetById(id);
                if (reader == null)
                {
                    Console.WriteLine("Читатель не найдена");
                    return;
                }

                Console.WriteLine($"Редактирование читателя: {reader.FullName}");
                Console.WriteLine("(Оставьте поле пустым, чтобы не изменять)");

                Console.Write($"ФИО [{reader.FullName}]: ");
                string fullName = Console.ReadLine();
                if (!string.IsNullOrEmpty(fullName))
                    reader.FullName = fullName;

                Console.Write($"Номер билета [{reader.TicketNumber}]: ");
                string ticket = Console.ReadLine();
                if (!string.IsNullOrEmpty(ticket))
                    reader.TicketNumber = ticket;

                Console.Write($"Контактная информация [{reader.ContactInfo}]: ");
                string contact = Console.ReadLine();
                if (contact != "")
                    reader.ContactInfo = contact;

                _readerService.Update(reader);
                Console.WriteLine("Читатель успешно обновлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void DeleteReader()
        {
            Console.WriteLine("\n=== УДАЛЕНИЕ ЧИТАТЕЛЯ ===");

            try
            {
                Console.Write("Введите ID читателя для удаления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Неверный формат ID");
                    return;
                }

                Console.Write("Вы уверены? (да/нет): ");
                string confirm = Console.ReadLine();

                if (confirm?.ToLower() == "да")
                {
                    _readerService.Delete(id);
                    Console.WriteLine("Читатель успешно удален!");
                }
                else
                {
                    Console.WriteLine("Удаление отменено");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void SearchReader()
        {
            Console.WriteLine("\n=== ПОИСК ЧИТАТЕЛЯ ===");

            Console.Write("Введите поисковый запрос: ");
            string searchTerm = Console.ReadLine();

            if (string.IsNullOrEmpty(searchTerm))
            {
                Console.WriteLine("Поисковый запрос не может быть пустым");
                return;
            }

            var readers = _readerService.SearchReaders(searchTerm);

            if (!readers.Any())
            {
                Console.WriteLine("Читатели не найдены.");
                return;
            }

            Console.WriteLine("\nРезультаты поиска:");
            Console.WriteLine("{0,-5} {1,-30} {2,-15} {3,-30}",
                "ID", "ФИО", "Билет", "Контакты");
            Console.WriteLine(new string('-', 80));

            foreach (var reader in readers)
            {
                Console.WriteLine("{0,-5} {1,-30} {2,-15} {3,-30}",
                    reader.Id,
                    Truncate(reader.FullName, 28),
                    reader.TicketNumber,
                    Truncate(reader.ContactInfo ?? "нет", 28));
            }
        }

        static void GiveBook()
        {
            Console.WriteLine("\n=== ВЫДАЧА КНИГИ ===");

            try
            {
                // Показываем всех читателей
                ShowAllReaders();
                Console.Write("\nВведите ID читателя: ");
                if (!int.TryParse(Console.ReadLine(), out int readerId))
                {
                    Console.WriteLine("Неверный формат ID читателя");
                    return;
                }

                // Показываем доступные книги
                var availableBooks = _bookService.SearchBooks("Доступна", "status");
                if (!availableBooks.Any())
                {
                    Console.WriteLine("Нет доступных книг");
                    return;
                }

                Console.WriteLine("\nДоступные книги:");
                Console.WriteLine("{0,-5} {1,-30} {2,-25}",
                    "ID", "Название", "Автор");
                Console.WriteLine(new string('-', 60));

                foreach (var book in availableBooks)
                {
                    Console.WriteLine("{0,-5} {1,-30} {2,-25}",
                        book.Id,
                        Truncate(book.Title, 28),
                        Truncate(book.Author, 23));
                }

                Console.Write("\nВведите ID книги: ");
                if (!int.TryParse(Console.ReadLine(), out int bookId))
                {
                    Console.WriteLine("Неверный формат ID книги");
                    return;
                }

                _loanService.GiveBook(bookId, readerId);
                Console.WriteLine("Книга успешно выдана!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void ReturnBook()
        {
            Console.WriteLine("\n=== ВОЗВРАТ КНИГИ ===");

            try
            {
                // Показываем активные выдачи
                var activeLoans = _loanService.GetActiveLoans();
                if (!activeLoans.Any())
                {
                    Console.WriteLine("Нет активных выдач");
                    return;
                }

                Console.WriteLine("Активные выдачи:");
                Console.WriteLine("{0,-5} {1,-30} {2,-30} {3,-15} {4,-15}",
                    "ID", "Книга", "Читатель", "Дата выдачи", "Срок возврата");
                Console.WriteLine(new string('-', 95));

                foreach (var loan in activeLoans)
                {
                    var book = _bookService.GetById(loan.BookId);
                    var reader = _readerService.GetById(loan.ReaderId);

                    Console.WriteLine("{0,-5} {1,-30} {2,-30} {3,-15:dd.MM.yyyy} {4,-15:dd.MM.yyyy}",
                        loan.Id,
                        Truncate(book?.Title ?? "неизвестно", 28),
                        Truncate(reader?.FullName ?? "неизвестно", 28),
                        loan.LoanDate,
                        loan.ReturnDate);
                }

                Console.Write("\nВведите ID выдачи для возврата: ");
                if (!int.TryParse(Console.ReadLine(), out int loanId))
                {
                    Console.WriteLine("Неверный формат ID");
                    return;
                }

                _loanService.ReturnBook(loanId);
                Console.WriteLine("Книга успешно возвращена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void ReserveBook()
        {
            Console.WriteLine("\n=== РЕЗЕРВИРОВАНИЕ КНИГИ ===");

            try
            {
                // Проверяем и отменяем просроченные резервы
                _reservationService.CancelExpiredReservations();

                // Показываем всех читателей
                ShowAllReaders();
                Console.Write("\nВведите ID читателя: ");
                if (!int.TryParse(Console.ReadLine(), out int readerId))
                {
                    Console.WriteLine("Неверный формат ID читателя");
                    return;
                }

                // Показываем доступные книги
                var availableBooks = _bookService.SearchBooks("Доступна", "status");
                if (!availableBooks.Any())
                {
                    Console.WriteLine("Нет доступных книг для резервирования");
                    return;
                }

                Console.WriteLine("\nДоступные книги для резервирования:");
                Console.WriteLine("{0,-5} {1,-30} {2,-25}",
                    "ID", "Название", "Автор");
                Console.WriteLine(new string('-', 60));

                foreach (var book in availableBooks)
                {
                    Console.WriteLine("{0,-5} {1,-30} {2,-25}",
                        book.Id,
                        Truncate(book.Title, 28),
                        Truncate(book.Author, 23));
                }

                Console.Write("\nВведите ID книги: ");
                if (!int.TryParse(Console.ReadLine(), out int bookId))
                {
                    Console.WriteLine("Неверный формат ID книги");
                    return;
                }

                _reservationService.Reserve(bookId, readerId);
                Console.WriteLine("Книга успешно зарезервирована на 3 дня!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void ShowFinesMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== УПРАВЛЕНИЕ ШТРАФАМИ ===");
                Console.WriteLine("1. Просмотр всех штрафов");
                Console.WriteLine("2. Просмотр неоплаченных штрафов");
                Console.WriteLine("3. Оплатить штраф");
                Console.WriteLine("4. Просмотр задолженности читателя");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllFines();
                        break;
                    case "2":
                        ShowUnpaidFines();
                        break;
                    case "3":
                        PayFine();
                        break;
                    case "4":
                        ShowReaderDebt();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }

        static void ShowAllFines()
        {
            Console.WriteLine("\n=== ВСЕ ШТРАФЫ ===");
            var fines = _fineService.GetAllFines();

            if (!fines.Any())
            {
                Console.WriteLine("Штрафы не найдены.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-30} {2,-15} {3,-10} {4,-10} {5,-15}",
                "ID", "Читатель", "Сумма", "Оплачен", "Дата", "Книга");
            Console.WriteLine(new string('-', 85));

            foreach (var fine in fines)
            {
                var reader = _readerService.GetById(fine.ReaderId);
                var loan = _context.Loans.Find(fine.LoanId);
                var book = loan != null ? _bookService.GetById(loan.BookId) : null;

                Console.WriteLine("{0,-5} {1,-30} {2,-15:C} {3,-10} {4,-15:dd.MM.yyyy} {5,-30}",
                    fine.Id,
                    Truncate(reader?.FullName ?? "неизвестно", 28),
                    fine.Amount,
                    fine.IsPaid ? "Да" : "Нет",
                    fine.CreatedDate,
                    Truncate(book?.Title ?? "неизвестно", 28));
            }
        }

        static void ShowUnpaidFines()
        {
            Console.WriteLine("\n=== НЕОПЛАЧЕННЫЕ ШТРАФЫ ===");
            var fines = _fineService.GetUnpaidFines();

            if (!fines.Any())
            {
                Console.WriteLine("Неоплаченные штрафы не найдены.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-30} {2,-15} {3,-15}",
                "ID", "Читатель", "Сумма", "Дата");
            Console.WriteLine(new string('-', 65));

            foreach (var fine in fines)
            {
                var reader = _readerService.GetById(fine.ReaderId);

                Console.WriteLine("{0,-5} {1,-30} {2,-15:C} {3,-15:dd.MM.yyyy}",
                    fine.Id,
                    Truncate(reader?.FullName ?? "неизвестно", 28),
                    fine.Amount,
                    fine.CreatedDate);
            }
        }

        static void PayFine()
        {
            Console.WriteLine("\n=== ОПЛАТА ШТРАФА ===");

            try
            {
                ShowUnpaidFines();

                Console.Write("\nВведите ID штрафа для оплаты: ");
                if (!int.TryParse(Console.ReadLine(), out int fineId))
                {
                    Console.WriteLine("Неверный формат ID");
                    return;
                }

                _fineService.MarkAsPaid(fineId);
                Console.WriteLine("Штраф успешно оплачен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void ShowReaderDebt()
        {
            Console.WriteLine("\n=== ЗАДОЛЖЕННОСТЬ ЧИТАТЕЛЯ ===");

            ShowAllReaders();

            Console.Write("\nВведите ID читателя: ");
            if (!int.TryParse(Console.ReadLine(), out int readerId))
            {
                Console.WriteLine("Неверный формат ID");
                return;
            }

            var reader = _readerService.GetById(readerId);
            if (reader == null)
            {
                Console.WriteLine("Читатель не найден");
                return;
            }

            decimal totalDebt = _fineService.GetTotalDebt(readerId);
            Console.WriteLine($"\nОбщая задолженность {reader.FullName}: {totalDebt:C}");

            if (totalDebt > 0)
            {
                var fines = _fineService.GetFinesByReader(readerId).Where(f => !f.IsPaid);
                Console.WriteLine("Состав задолженности:");

                foreach (var fine in fines)
                {
                    var loan = _context.Loans.Find(fine.LoanId);
                    var book = loan != null ? _bookService.GetById(loan.BookId) : null;

                    Console.WriteLine($"  - {book?.Title ?? "неизвестно"}: {fine.Amount:C} (создан: {fine.CreatedDate:dd.MM.yyyy})");
                }
            }
        }

        static void SearchBooks()
        {
            Console.WriteLine("\n=== ПОИСК КНИГ ===");

            Console.WriteLine("Поиск по:");
            Console.WriteLine("1. Названию");
            Console.WriteLine("2. Автору");
            Console.WriteLine("3. Жанру");
            Console.WriteLine("4. ISBN");
            Console.WriteLine("5. Статусу");
            Console.Write("Выберите критерий поиска: ");

            string searchBy = Console.ReadLine() switch
            {
                "1" => "title",
                "2" => "author",
                "3" => "genre",
                "4" => "isbn",
                "5" => "status",
                _ => "title"
            };

            Console.Write("Введите поисковый запрос: ");
            string searchTerm = Console.ReadLine();

            if (string.IsNullOrEmpty(searchTerm))
            {
                Console.WriteLine("Поисковый запрос не может быть пустым");
                return;
            }

            var books = _bookService.SearchBooks(searchTerm, searchBy);

            if (!books.Any())
            {
                Console.WriteLine("Книги не найдены.");
                return;
            }

            Console.WriteLine($"\nРезультаты поиска ({searchTerm}):");
            Console.WriteLine("{0,-5} {1,-30} {2,-25} {3,-15} {4,-8} {5,-8} {6,-15}",
                "ID", "Название", "Автор", "Жанр", "Доступно", "Всего", "Статус");
            Console.WriteLine(new string('-', 110));

            foreach (var book in books)
            {
                Console.WriteLine("{0,-5} {1,-30} {2,-25} {3,-15} {4,-8} {5,-8} {6,-15}",
                    book.Id,
                    Truncate(book.Title, 28),
                    Truncate(book.Author, 23),
                    Truncate(book.Genre, 13),
                    book.AvailableCount,
                    book.TotalCount,
                    book.Status);
            }
        }

        static void ShowActiveLoans()
        {
            Console.WriteLine("\n=== АКТИВНЫЕ ВЫДАЧИ ===");
            var loans = _loanService.GetActiveLoans();

            if (!loans.Any())
            {
                Console.WriteLine("Активные выдачи не найдены.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-30} {2,-30} {3,-15} {4,-15} {5,-10}",
                "ID", "Книга", "Читатель", "Дата выдачи", "Срок возврата", "Дней осталось");
            Console.WriteLine(new string('-', 105));

            foreach (var loan in loans)
            {
                var book = _bookService.GetById(loan.BookId);
                var reader = _readerService.GetById(loan.ReaderId);
                int daysLeft = (loan.ReturnDate - DateTime.Now).Days;
                string daysLeftStr = daysLeft >= 0 ? daysLeft.ToString() : $"просрочено {-daysLeft}";

                Console.WriteLine("{0,-5} {1,-30} {2,-30} {3,-15:dd.MM.yyyy} {4,-15:dd.MM.yyyy} {5,-10}",
                    loan.Id,
                    Truncate(book?.Title ?? "неизвестно", 28),
                    Truncate(reader?.FullName ?? "неизвестно", 28),
                    loan.LoanDate,
                    loan.ReturnDate,
                    daysLeftStr);
            }
        }

        static void ShowActiveReservations()
        {
            Console.WriteLine("\n=== АКТИВНЫЕ РЕЗЕРВЫ ===");
            var reservations = _reservationService.GetActiveReservations();

            if (!reservations.Any())
            {
                Console.WriteLine("Активные резервы не найдены.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-30} {2,-30} {3,-15} {4,-15} {5,-10}",
                "ID", "Книга", "Читатель", "Дата резерва", "Действителен до", "Дней осталось");
            Console.WriteLine(new string('-', 105));

            foreach (var reservation in reservations)
            {
                var book = _bookService.GetById(reservation.BookId);
                var reader = _readerService.GetById(reservation.ReaderId);
                int daysLeft = (reservation.ExpirationDate - DateTime.Now).Days;
                string daysLeftStr = daysLeft >= 0 ? daysLeft.ToString() : "истек";

                Console.WriteLine("{0,-5} {1,-30} {2,-30} {3,-15:dd.MM.yyyy} {4,-15:dd.MM.yyyy} {5,-10}",
                    reservation.Id,
                    Truncate(book?.Title ?? "неизвестно", 28),
                    Truncate(reader?.FullName ?? "неизвестно", 28),
                    reservation.ReservationDate,
                    reservation.ExpirationDate,
                    daysLeftStr);
            }
        }

        // Вспомогательный метод для обрезания строк
        static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
        }
    }
}