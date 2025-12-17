using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Library.Models
{
    [Table("reservations")]
    public class Reservations     //бронь
    {
        [Key][Column("id")] public int id { get; set; }
        [ForeignKey("Book")][Column("book_id")] public int BookId { get; set; }     //связь какую книгу бронь
        [ForeignKey("Reader")][Column("reader_id")] public int ReaderId { get; set; } //связь кто бронирует
        [Column("reservation_date")] public DateTime ReservationDate { get; set; }      //дата бронирования
        [Column("expiration_date")] public DateTime ExpirationDate { get; set; }        //дата когда надо вернуть
        [Column("is_active")] public bool IsActive { get; set; } = true;        // активна ли бронь true ждет fale отменена или просрочена


    }
}
