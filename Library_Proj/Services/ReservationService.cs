using Library_Proj.Data.Repositories;
using Library_Proj.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Library_Proj.Services
{
    public class ReservationService
    {
        private readonly IRepository<Reservations> _resRepo;

        public ReservationService(IRepository<Reservations> resRepo)
        {
            _resRepo = resRepo;
        }

        public void Reserve(int bookId, int readerId)
        {
            //предотвращение двух резервов на 1 книгу
            bool exists = _resRepo.Find(r => r.BookId == bookId && r.ReaderId == readerId && r.IsActive).Any();

            if (exists)
                throw new Exception("резерв уже существует");

            var reservation = new Reservations
            {
                BookId = bookId,
                ReaderId = readerId,
                IsActive = true,
                ReservationDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddDays(3),
            };

            _resRepo.Add(reservation);
            _resRepo.SaveChanges();
        }
    }
}
