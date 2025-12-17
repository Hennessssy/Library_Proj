using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Library.Models
{
    [Table("fines")]
    public class Fine       //долги
    {
        [Key][Column("id")] public int Id { get; set; }
        [ForeignKey("Reader")][Column("reader_id")] public int ReaderId { get; set; }   //связь кому выписан штраф
        public Reader Reader { get; set; }  //навигационное свойство
        [ForeignKey("Loan")][Column("loan_id")] public int LoanId { get; set; }         //связь за какую выдачу штраф
        public Loan Loan { get; set; } //навигационное свойство
        [Column("amount")] public decimal Amount { get; set; }                          //сумма штрафа
        [Column("is_paid")] public bool IsPaid { get; set; } = false;                   //оплачен ли штраф false net true da
        [Column("created_date")] public DateTime CreatedDate { get; set; } = DateTime.Now;     //дата начисления штрафа


    }
}
