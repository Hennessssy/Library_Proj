using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;        //для валидации данных типа key required
using System.ComponentModel.DataAnnotations.Schema; //для изменения названия в самой бд типа Table books

namespace Library.Models
{

    [Table("books")] //в самой бд будет называться books а тут класс будет Book
    public class Book //класс книга
    {
        [Key][Column("id")] public int Id { get; set; } //первичный ключ 
        [Required][StringLength(250)][Column("title")] public string Title { get; set; } = string.Empty; //поле не может быть null
        [Required][StringLength(100)][Column("author")] public string Author { get; set; } = string.Empty;
        [Required][StringLength(50)][Column("genre")] public string Genre { get; set; } = string.Empty;
        [StringLength(15)][Column("isbn")] public string? ISBN { get; set; }
        [Column("year")] public int Year { get; set; }
        [Required][Column("total_count")] public int TotalCount { get; set; } = 0;
        [Required][Column("available_count")] public int AvailableCount { get; set; } = 0;
        [Required][StringLength(30)][Column("status")] public string Status { get; set; } = "Доступна"; //по умолчанию доступна

    }
}