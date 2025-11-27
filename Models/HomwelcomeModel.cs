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
        public string Header { get; set; } = string.Empty;

        [Required]
        public string SubContent { get; set; } = string.Empty;

       
        public byte[]? Image1 { get; set; }

        
        public byte[]? Image2 { get; set; }

        [Required]
        public int loan_id { get; set; }
    }
}
