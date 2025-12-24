using System;
using System.Collections.Generic;
using System.Linq;
using Library_Proj.Data.Repositories;
using Library_Proj.Models;

namespace Library_Proj.Services
{
    public class FineService
    {
        private readonly IRepository<Fine> _fineRepo;
        private readonly IRepository<Reader> _readerRepo;

        private const decimal DailyFine = 100m;

        public FineService(IRepository<Fine> fineRepo, IRepository<Reader> readerRepo)
        {
            _fineRepo = fineRepo;
            _readerRepo = readerRepo;
        }

        public void CreateFine(Loan loan)
        {
            if (!loan.ActualReturnDate.HasValue)
                throw new Exception("Дата возврата не установлена");

            int daysLate = (loan.ActualReturnDate.Value - loan.ReturnDate).Days;

            if (daysLate <= 0)
                return;

            var fine = new Fine
            {
                LoanId = loan.Id,
                ReaderId = loan.ReaderId,
                Amount = daysLate * DailyFine,
                IsPaid = false,
                CreatedDate = DateTime.Now
            };

            var reader = _readerRepo.GetById(loan.ReaderId);
            reader.HasDebt = true;

            _fineRepo.Add(fine);
            _readerRepo.Update(reader);

            _fineRepo.SaveChanges();
        }

        public IEnumerable<Fine> GetAllFines()
        {
            return _fineRepo.GetAll();
        }

        public IEnumerable<Fine> GetFinesByReader(int readerId)
        {
            return _fineRepo.Find(f => f.ReaderId == readerId);
        }

        public IEnumerable<Fine> GetUnpaidFines()
        {
            return _fineRepo.Find(f => !f.IsPaid);
        }

        public void MarkAsPaid(int fineId)
        {
            var fine = _fineRepo.GetById(fineId);
            if (fine == null)
                throw new Exception("Штраф не найден");

            fine.IsPaid = true;

            // Проверяем, остались ли у читателя неоплаченные штрафы
            var readerFines = _fineRepo.Find(f => f.ReaderId == fine.ReaderId && !f.IsPaid);
            if (!readerFines.Any())
            {
                var reader = _readerRepo.GetById(fine.ReaderId);
                reader.HasDebt = false;
                _readerRepo.Update(reader);
            }

            _fineRepo.Update(fine);
            _fineRepo.SaveChanges();
        }

        public decimal GetTotalDebt(int readerId)
        {
            var fines = _fineRepo.Find(f => f.ReaderId == readerId && !f.IsPaid);
            return fines.Sum(f => f.Amount);
        }
    }
}