using System;
using System.Collections.Generic;
using System.Text;
using Library_Proj.Data.Repositories;
using Library_Proj.Models;


namespace Library_Proj.Services
{
    public class ReaderService  //бизнес логика для работы с читателяи
    {
        private readonly IRepository<Reader> _readerRepo;

        public ReaderService(IRepository<Reader> readerRepo)
        {
            _readerRepo = readerRepo;
        }

        public void Add(Reader reader)
        {
            //номер читателя должен быть уникален
            bool exists = _readerRepo.Find(r =>r.TicketNumber == reader.TicketNumber).Any();

            if (exists)
                throw new Exception("читатель с таким билетом уже существует");

            reader.HasDebt = false;

            _readerRepo.Add(reader);
            _readerRepo.SaveChanges();
        }
    }
}
