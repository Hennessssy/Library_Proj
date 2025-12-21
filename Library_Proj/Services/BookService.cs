using System;
using System.Collections.Generic;
using System.Text;
using Library_Proj.Data.Repositories;
using Library_Proj.Models;

namespace Library_Proj.Services
{
    //бизнес логика для работы с книгами
    public class BookService
    {
        private readonly IRepository<Book> _bookRepo;

        public BookService(IRepository<Book> bookRepo)
        {
            _bookRepo = bookRepo;
        }

        public IEnumerable<Book> GetAll()   //получить все книги
        {
            return _bookRepo.GetAll();
        }

        public void Add(Book book)
        {
            if (book.TotalCount < -0)
                throw new Exception("общее количество книг должно быть больше нуля");

            book.AvailableCount = book.TotalCount;
            book.Status = "Доступна";

            _bookRepo.Add(book);
            _bookRepo.SaveChanges();
        }

        public void Update(Book book)
        {
            if (book.AvailableCount > book.TotalCount)
                throw new Exception("Доступных книг больше чем всего");

            //пересчет статуса
            book.Status = book.AvailableCount > 0
                ? "Доступна"
                : "Выдана";

            _bookRepo.Update(book);
            _bookRepo.SaveChanges();
        }
    }
}
