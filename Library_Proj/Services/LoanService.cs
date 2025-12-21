using Library_Proj.Data.Repositories;
using Library_Proj.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Proj.Services
{
    public class LoanService    //бизнес логика для работы с выдачей и возвратом книг
    {
        private readonly IRepository<Loan> _loanRepo;
        private readonly IRepository<Book> _bookRepo;
        private readonly IRepository<Reader> _readerRepo;

        public LoanService(IRepository<Loan> loanRepo,IRepository<Book> bookRepo,IRepository<Reader> readerRepo)
        {
            _loanRepo = loanRepo;
            _bookRepo = bookRepo;
            _readerRepo = readerRepo;
        }

        public void GiveBook(int bookId, int readerId)
        {
            var book = _bookRepo.GetById(bookId);
            var reader = _readerRepo.GetById(readerId);

            if (book == null)
                throw new Exception("книга не найдена");
            if (reader == null)
                throw new Exception("читатель не найден");
            if (book.AvailableCount <= 0)
                throw new Exception("книга недоступна");
            if (reader.HasDebt)
                throw new Exception("у читателя есть задолжность");

            var loan = new Loan()
            {
                BookId = bookId,
                ReaderId = readerId,
                LoanDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(14)
            };

            book.AvailableCount--;
            book.Status = book.AvailableCount > 0 ? "Доступна" : "Выдана";

            _loanRepo.Add(loan);
            _bookRepo.Update(book);

            _loanRepo.SaveChanges();
        }

        public void ReturnBook(int loanId)
        {
            var loan = _loanRepo.GetById(loanId);

            if (loan == null)
                throw new Exception("выдача не найдена");
            if (loan.ActualReturnDate != null)
                throw new Exception("книга уже возвращена");

            loan.ActualReturnDate = DateTime.Now;

            var book = _bookRepo.GetById(loan.BookId);
            book.AvailableCount++;
            book.Status = "Доступна";

            _loanRepo.Update(loan);
            _bookRepo.Update(book);

            _loanRepo.SaveChanges();
        }
    }
}
