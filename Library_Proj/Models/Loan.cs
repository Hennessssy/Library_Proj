using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Library_Proj.Models
{
    [Table("loans")]
    public class Loan
    {
        [Key][Column("id")] public int Id { get; set; }

        [Column("book_id")] public int BookId { get; set; }
        [ForeignKey("BookId")] public Book Book { get; set; } // навигация к книге

        [Column("reader_id")] public int ReaderId { get; set; }
        [ForeignKey("ReaderId")] public Reader Reader { get; set; } // навигация к читателю

        [Column("loan_date")] public DateTime LoanDate { get; set; }    //дата выдачи
        [Column("return_date")] public DateTime ReturnDate { get; set; }    //дата возврата плановая
        [Column("actual_return_date")] public DateTime? ActualReturnDate { get; set; }  //дата возврата по факту 
    }
}