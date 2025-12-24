using System;
using System.Collections.Generic;
using System.Linq;
using Library_Proj.Data.Repositories;
using Library_Proj.Models;

namespace Library_Proj.Services
{
    //бизнес логика для работы с книгами
    public class BookService
    {
        private readonly IRepository<Book> _bookRepo;
        private readonly IRepository<Reservations> _reservationRepo;

        public BookService(IRepository<Book> bookRepo, IRepository<Reservations> reservationRepo = null)
        {
            _bookRepo = bookRepo;
            _reservationRepo = reservationRepo;
        }

        public IEnumerable<Book> GetAll()   //получить все книги
        {
            return _bookRepo.GetAll();
        }

        public Book GetById(int id)
        {
            return _bookRepo.GetById(id);
        }

        public void Add(Book book)
        {
            if (book.TotalCount <= 0)
                throw new Exception("Общее количество книг должно быть больше нуля");

            // Проверка уникальности ISBN, если указан
            if (!string.IsNullOrEmpty(book.ISBN))
            {
                bool isbnExists = _bookRepo.Find(b => b.ISBN == book.ISBN).Any();
                if (isbnExists)
                    throw new Exception("Книга с таким ISBN уже существует");
            }

            book.AvailableCount = book.TotalCount;
            book.Status = "Доступна";

            _bookRepo.Add(book);
            _bookRepo.SaveChanges();
        }

        public void Update(Book book)
        {
            if (book.AvailableCount > book.TotalCount)
                throw new Exception("Доступных книг больше чем всего");

            // Пересчет статуса с учетом резервирований
            if (book.AvailableCount > 0)
            {
                // Проверяем, есть ли активные резервы на эту книгу
                bool hasActiveReservation = _reservationRepo != null &&
                    _reservationRepo.Find(r => r.BookId == book.Id && r.IsActive).Any();

                book.Status = hasActiveReservation ? "Зарезервирована" : "Доступна";
            }
            else
            {
                book.Status = "Выдана";
            }

            _bookRepo.Update(book);
            _bookRepo.SaveChanges();
        }

        public void Delete(int id)
        {
            var book = _bookRepo.GetById(id);
            if (book == null)
                throw new Exception("Книга не найдена");

            // Проверяем, можно ли удалить книгу
            if (book.AvailableCount < book.TotalCount)
                throw new Exception("Нельзя удалить книгу, так как она выдана читателям");

            _bookRepo.Remove(book);
            _bookRepo.SaveChanges();
        }

        // Поиск книг по различным параметрам
        public IEnumerable<Book> SearchBooks(string searchTerm, string searchBy = "title")
        {
            var allBooks = _bookRepo.GetAll();

            return searchBy.ToLower() switch
            {
                "title" => allBooks.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)),
                "author" => allBooks.Where(b => b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)),
                "genre" => allBooks.Where(b => b.Genre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)),
                "isbn" => allBooks.Where(b => b.ISBN != null && b.ISBN.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)),
                "status" => allBooks.Where(b => b.Status.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)),
                _ => allBooks.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                        b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            };
        }
    }
}