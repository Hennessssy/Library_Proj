using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Library_Proj.Models;

namespace Library_Proj.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<Reservations> Reservations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=library.db");
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            //книги
            model.Entity<Book>().HasData(
                new Book { Id = 1, Title = "1984", Author = "джордж оруэлл", Genre = "антиутопия", Year = 1949, TotalCount = 5, AvailableCount = 5, Status = "Доступна" },
                new Book { Id = 2, Title = "скотный двор", Author = "джордж оруэлл", Genre = "сатира", Year = 1945, TotalCount = 4, AvailableCount = 4, Status = "Доступна" },
                new Book { Id = 3, Title = "война и мир", Author = "лев толстой", Genre = "роман", Year = 1869, TotalCount = 3, AvailableCount = 3, Status = "Доступна" },
                new Book { Id = 4, Title = "преступление и наказание", Author = "фёдор достоевский", Genre = "роман", Year = 1866, TotalCount = 6, AvailableCount = 6, Status = "Доступна" },
                new Book { Id = 5, Title = "белый клык", Author = "джек лондон", Genre = "приключения", Year = 1906, TotalCount = 5, AvailableCount = 5, Status = "Доступна" }
                );
            //читатели
            model.Entity<Reader>().HasData(
                new Reader { Id = 1, FullName = "иванов иван иванович", TicketNumber = "r001", HasDebt = false },
                new Reader { Id = 2, FullName = "петров петр петрович", TicketNumber = "r002", HasDebt = false },
                new Reader { Id = 3, FullName = "сидоров сергей сергеевич", TicketNumber = "r003", HasDebt = false }
                );

            base.OnModelCreating(model);
        }

    }
}
