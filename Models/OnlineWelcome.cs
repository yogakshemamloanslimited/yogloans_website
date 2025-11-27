using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yogloansdotnet.Models
{
    [Table("OnlineWelcome")] // Optional: maps to this table name in DB
    public class OnlineWelcome
    {
        [Key]
        public int Id { get; set; }

       
        public byte[]? Image1 { get; set; }

         
        public byte[]? Image2 { get; set; }

        [Required]
        public string Mainhead { get; set; } = string.Empty;

         [Required]
        public string Subhead { get; set; } = string.Empty;
    }
}
