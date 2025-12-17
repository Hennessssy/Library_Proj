using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Library_Proj.Models
{
    [Table("reservations")]
    public class Reservations
    {
        [Key][Column("id")] public int Id { get; set; }

        [Column("book_id")] public int BookId { get; set; }     //какую книгу бронирует
        [ForeignKey("BookId")] public Book Book { get; set; }

        [Column("reader_id")] public int ReaderId { get; set; }     //кто бронирует 
        [ForeignKey("ReaderId")] public Reader Reader { get; set; }

        [Column("reservation_date")] public DateTime ReservationDate { get; set; }  //дата бронирования
        [Column("expiration_date")] public DateTime ExpirationDate { get; set; }    //дата окончания брони
        [Column("is_active")] public bool IsActive { get; set; } = true;
    }
}
