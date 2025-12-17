using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Library.Models
{
    [Table("loans")]
    public class Loan   //выдачи (кто какую книгу взял и когда вернет)
    {
        [Key][Column("id")] public int Id { get; set; }
        [ForeignKey("Book")] //связь какая книга выдана
        [Column("book_id")] public int BookId { get; set; }
        [ForeignKey("Reader")] //связь кто взял книгу
        public Book Book { get; set; } //навигационное свойство
        [Column("reader_id")] public int ReaderId { get; set; }
        [Column("loan_date")] public DateTime LoanDate { get; set; }        //дата выдачи
        [Column("return_date")] public DateTime ReturnDate { get; set; }    //дата возврата(когда надо вернуть)
        [Column("actual_return_date")] public DateTime? ActualReturnDate { get; set; } // фактическая дата возврата


    }
}
