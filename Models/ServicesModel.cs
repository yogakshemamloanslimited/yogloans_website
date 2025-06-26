using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yogloansdotnet.Models;

namespace yogloansdotnet.Models
{
    [Table("Services")]
    public class ServicesModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        
        [Required]
        public string Subtitle { get; set; } = string.Empty;

        [Required]
        [Pdf]
        public string FilePath { get; set; } = string.Empty;
    }
} 