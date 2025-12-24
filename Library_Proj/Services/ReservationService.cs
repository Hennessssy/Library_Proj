using System;
using System.Collections.Generic;
using System.Linq;
using Library_Proj.Data.Repositories;
using Library_Proj.Models;

namespace Library_Proj.Services
{
    public class ReservationService
    {
        private readonly IRepository<Reservations> _resRepo;
        private readonly IRepository<Book> _bookRepo;

        public ReservationService(IRepository<Reservations> resRepo, IRepository<Book> bookRepo = null)
        {
            _resRepo = resRepo;
            _bookRepo = bookRepo;
        }

        public void Reserve(int bookId, int readerId)
        {
            var book = _bookRepo?.GetById(bookId);
            if (book == null)
                throw new Exception("Книга не найдена");

            if (book.AvailableCount <= 0)
                throw new Exception("Нет доступных экземпляров для резервирования");

            // Проверяем, не зарезервирована ли уже книга этим читателем
            bool exists = _resRepo.Find(r =>
                r.BookId == bookId &&
                r.ReaderId == readerId &&
                r.IsActive).Any();

            if (exists)
                throw new Exception("Резерв уже существует");

            var reservation = new Reservations
            {
                BookId = bookId,
                ReaderId = readerId,
                IsActive = true,
                ReservationDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddDays(3),
            };

            // Обновляем статус книги
            book.Status = "Зарезервирована";
            _bookRepo?.Update(book);

            _resRepo.Add(reservation);
            _resRepo.SaveChanges();
        }

        public IEnumerable<Reservations> GetAllReservations()
        {
            return _resRepo.GetAll();
        }

        public IEnumerable<Reservations> GetActiveReservations()
        {
            return _resRepo.Find(r => r.IsActive);
        }

        public void CancelReservation(int reservationId)
        {
            var reservation = _resRepo.GetById(reservationId);
            if (reservation == null)
                throw new Exception("Резервирование не найдено");

            reservation.IsActive = false;

            // Обновляем статус книги
            var book = _bookRepo?.GetById(reservation.BookId);
            if (book != null)
            {
                if (book.AvailableCount > 0)
                {
                    book.Status = "Доступна";
                    _bookRepo.Update(book);
                }
            }

            _resRepo.Update(reservation);
            _resRepo.SaveChanges();
        }

        // Автоматическая отмена просроченных резервов
        public void CancelExpiredReservations()
        {
            var expiredReservations = _resRepo.Find(r =>
                r.ExpirationDate < DateTime.Now &&
                r.IsActive);

            foreach (var reservation in expiredReservations)
            {
                CancelReservation(reservation.Id);
            }
        }
    }
}