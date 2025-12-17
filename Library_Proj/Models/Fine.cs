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

        [Column("reader_id")] public int ReaderId { get; set; }     //кому выписан штраф
        [ForeignKey("ReaderId")] public Reader Reader { get; set; } //навигационное свойство

        [Column("loan_id")] public int LoanId { get; set; }     //за какую выдачу штраф
        [ForeignKey("LoanId")] public Loan Loan { get; set; }   //навигационное свойство

        [Column("amount")] public decimal Amount { get; set; }          //сумма штрафа
        [Column("is_paid")] public bool IsPaid { get; set; } = false;       // выплачено или нет фолс нет тру да
        [Column("created_date")] public DateTime CreatedDate { get; set; } = DateTime.Now;  //дата начисления
    }
}
