using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Library_Proj.Models
{
    [Table("fines")]
    public class Fine
    {
        [Key][Column("id")] public int Id { get; set; }

        [Column("reader_id")] public int ReaderId { get; set; }
        [ForeignKey("ReaderId")] public Reader Reader { get; set; }

        [Column("loan_id")] public int LoanId { get; set; }
        [ForeignKey("LoanId")] public Loan Loan { get; set; }

        [Column("amount")] public decimal Amount { get; set; }
        [Column("is_paid")] public bool IsPaid { get; set; } = false;
        [Column("created_date")] public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
