using System;
using System.ComponentModel.DataAnnotations;

namespace LocationAPI.DAL.Models
{
    public class Transaction
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
