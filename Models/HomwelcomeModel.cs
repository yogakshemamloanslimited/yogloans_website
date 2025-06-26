using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yogloansdotnet.Models
{
    [Table("Homwelcome")] // Optional: maps to this table name in DB
    public class HomwelcomeModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string LoanType { get; set; } = string.Empty;

        [Required]
        public string Header { get; set; } = string.Empty;

        [Required]
        public string SubContent { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Image1 { get; set; }

        [StringLength(255)]
        public string? Image2 { get; set; }
    }
}
