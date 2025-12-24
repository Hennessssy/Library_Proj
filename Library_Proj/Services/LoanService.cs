using System;
using System.Collections.Generic;
using System.Linq;
using Library_Proj.Data.Repositories;
using Library_Proj.Models;

namespace Library_Proj.Services
{
    public class LoanService
    {
        private readonly IRepository<Loan> _loanRepo;
        private readonly IRepository<Book> _bookRepo;
        private readonly IRepository<Reader> _readerRepo;
        private readonly IRepository<Reservations> _reservationRepo;
        private readonly FineService _fineService;

        public LoanService(
            IRepository<Loan> loanRepo,
            IRepository<Book> bookRepo,
            IRepository<Reader> readerRepo,
            IRepository<Reservations> reservationRepo = null,
            FineService fineService = null)
        {
            _loanRepo = loanRepo;
            _bookRepo = bookRepo;
            _readerRepo = readerRepo;
            _reservationRepo = reservationRepo;
            _fineService = fineService;
        }

        public IEnumerable<Loan> GetAllLoans()
        {
            return _loanRepo.GetAll();
        }

        public IEnumerable<Loan> GetActiveLoans()
        {
            return _loanRepo.Find(l => l.ActualReturnDate == null);
        }

        public void GiveBook(int bookId, int readerId)
        {
            var book = _bookRepo.GetById(bookId);
            var reader = _readerRepo.GetById(readerId);

            if (book == null)
                throw new Exception("Книга не найдена");
            if (reader == null)
                throw new Exception("Читатель не найден");
            if (book.AvailableCount <= 0)
                throw new Exception("Книга недоступна");
            if (reader.HasDebt)
                throw new Exception("У читателя есть задолженность");

            // Проверяем, не зарезервирована ли книга другим читателем
            bool hasActiveReservation = _reservationRepo != null &&
                _reservationRepo.Find(r => r.BookId == bookId && r.IsActive && r.ReaderId != readerId).Any();

            if (hasActiveReservation)
                throw new Exception("Книга зарезервирована другим читателем");

            // Если была резервация текущим читателем - отменяем ее
            if (_reservationRepo != null)
            {
                var readerReservation = _reservationRepo
                    .Find(r => r.BookId == bookId && r.ReaderId == readerId && r.IsActive)
                    .FirstOrDefault();

                if (readerReservation != null)
                {
                    readerReservation.IsActive = false;
                    _reservationRepo.Update(readerReservation);
                }
            }

            var loan = new Loan()
            {
                BookId = bookId,
                ReaderId = readerId,
                LoanDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(14)
            };

            book.AvailableCount--;

            // Обновляем статус книги
            if (book.AvailableCount > 0)
            {
                // Проверяем, есть ли другие резервы
                bool hasOtherReservations = _reservationRepo != null &&
                    _reservationRepo.Find(r => r.BookId == bookId && r.IsActive).Any();

                book.Status = hasOtherReservations ? "Зарезервирована" : "Доступна";
            }
            else
            {
                book.Status = "Выдана";
            }

            _loanRepo.Add(loan);
            _bookRepo.Update(book);

            _loanRepo.SaveChanges();
        }

        public void ReturnBook(int loanId)
        {
            var loan = _loanRepo.GetById(loanId);

            if (loan == null)
                throw new Exception("Выдача не найдена");
            if (loan.ActualReturnDate != null)
                throw new Exception("Книга уже возвращена");

            loan.ActualReturnDate = DateTime.Now;

            var book = _bookRepo.GetById(loan.BookId);
            book.AvailableCount++;

            // Обновляем статус книги
            bool hasActiveReservation = _reservationRepo != null &&
                _reservationRepo.Find(r => r.BookId == book.Id && r.IsActive).Any();

            book.Status = hasActiveReservation ? "Зарезервирована" : "Доступна";

            // Начисляем штраф при необходимости
            if (_fineService != null && loan.ActualReturnDate > loan.ReturnDate)
            {
                _fineService.CreateFine(loan);

                // Обновляем статус читателя
                var reader = _readerRepo.GetById(loan.ReaderId);
                reader.HasDebt = true;
                _readerRepo.Update(reader);
            }

            _loanRepo.Update(loan);
            _bookRepo.Update(book);

            _loanRepo.SaveChanges();
        }

        public void ReturnBookByBookId(int bookId, int readerId)
        {
            // Находим активную выдачу для данной книги и читателя
            var loan = _loanRepo
                .Find(l => l.BookId == bookId &&
                          l.ReaderId == readerId &&
                          l.ActualReturnDate == null)
                .FirstOrDefault();

            if (loan == null)
                throw new Exception("Активная выдача не найдена");

            ReturnBook(loan.Id);
        }
    }
}