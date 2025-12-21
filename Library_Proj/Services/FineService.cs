using System;
using System.Collections.Generic;
using System.Text;
using Library_Proj.Data.Repositories;
using Library_Proj.Models;

namespace Library_Proj.Services
{
    public class FineService
    {
        private readonly IRepository<Fine> _fineRepo;
        private readonly IRepository<Reader> _readerRepo;

        private const decimal DailyFine = 100m;

        public FineService (IRepository<Fine> fineRepo, IRepository<Reader> rederRepo)
        {
            _fineRepo = fineRepo;
            _readerRepo = rederRepo;
        }

        public void CreateFine(Loan loan)
        {
            int daysLate = (loan.ActualReturnDate.Value - loan.ReturnDate).Days;

            if (daysLate <= 0)
                return;
            var fine = new Fine
            {
                LoanId = loan.Id,
                ReaderId = loan.ReaderId,
                Amount = daysLate * DailyFine,
                IsPaid = false
            };

            var reader = _readerRepo.GetById(loan.ReaderId);
            reader.HasDebt = true;

            _fineRepo.Add(fine);
            _readerRepo.Update(reader);

            _fineRepo.SaveChanges();
        }
    }
}
