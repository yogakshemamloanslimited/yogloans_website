using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yogloansdotnet.Models
{
    [Table("Loans")] // Optional: maps to this table name in DB
    public class LoanModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Loanname { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;
        [StringLength(255)]
        public string? icon { get; set; }

    
    
    }
}
