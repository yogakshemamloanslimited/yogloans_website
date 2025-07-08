using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yogloansdotnet.Models
{
    [Table("Nearby")] // Optional: maps to this table name in DB
    public class NearbyModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Image1 { get; set; } = string.Empty;

         [Required]
        public string Image2 { get; set; } = string.Empty;

        [Required]
        public string Mainhead { get; set; } = string.Empty;

         [Required]
        public string Subhead { get; set; } = string.Empty;
    }
}
