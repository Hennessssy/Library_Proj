using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Library.Models
{
    [Table("readers")]
    public class Reader
    {
        [Key][Column("id")] public int Id { get; set; }
        [Required][StringLength(100)][Column("full_name")] public string FullName { get; set; } = string.Empty; //фио полностью
        [Required][StringLength(25)][Column("tiket_number")] public string TicketNumber { get; set; } = string.Empty;
        [StringLength(200)][Column("contact_info")] public string? ContactInfo { get; set; }
        [Column("has_debt")] public bool HasDebt { get; set; } = false; //есть ли долги

    }
}
