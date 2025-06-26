using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yogloansdotnet.Models
{
    [Table("Loanspoints")] // Optional: maps to this table name in DB
    public class LoanPointModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Loan { get; set; } = string.Empty;

        [Required]
        public string Point { get; set; } = string.Empty;

        // This property is used for form binding only, not stored in database
        [NotMapped]
        public string[] Points { get; set; } = Array.Empty<string>();
    }
}
