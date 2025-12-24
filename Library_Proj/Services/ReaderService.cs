using System;
using System.Collections.Generic;
using System.Linq;
using Library_Proj.Data.Repositories;
using Library_Proj.Models;

namespace Library_Proj.Services
{
    public class ReaderService
    {
        private readonly IRepository<Reader> _readerRepo;

        public ReaderService(IRepository<Reader> readerRepo)
        {
            _readerRepo = readerRepo;
        }

        public IEnumerable<Reader> GetAll()
        {
            return _readerRepo.GetAll();
        }

        public Reader GetById(int id)
        {
            return _readerRepo.GetById(id);
        }

        public void Add(Reader reader)
        {
            // Номер читателя должен быть уникален
            bool exists = _readerRepo.Find(r => r.TicketNumber == reader.TicketNumber).Any();

            if (exists)
                throw new Exception("Читатель с таким билетом уже существует");

            reader.HasDebt = false;

            _readerRepo.Add(reader);
            _readerRepo.SaveChanges();
        }

        public void Update(Reader reader)
        {
            _readerRepo.Update(reader);
            _readerRepo.SaveChanges();
        }

        public void Delete(int id)
        {
            var reader = _readerRepo.GetById(id);
            if (reader == null)
                throw new Exception("Читатель не найден");

            // Проверяем, есть ли у читателя активные выдачи или долги
            if (reader.HasDebt)
                throw new Exception("Нельзя удалить читателя с задолженностью");

            _readerRepo.Remove(reader);
            _readerRepo.SaveChanges();
        }

        // Поиск читателей
        public IEnumerable<Reader> SearchReaders(string searchTerm)
        {
            return _readerRepo.Find(r =>
                r.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                r.TicketNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.ContactInfo != null && r.ContactInfo.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            );
        }
    }
}