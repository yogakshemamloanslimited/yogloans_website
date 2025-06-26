using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yogloansdotnet.Models
{
    [Table("Counts")] // Optional: maps to this table name in DB
    public class CountsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Head { get; set; } = string.Empty;

        [Required]
        public string Counts { get; set; } = string.Empty;

    
    }
}
