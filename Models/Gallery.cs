using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yogloansdotnet.Attributes; // Include your custom attribute

namespace yogloansdotnet.Models
{
     [Table("Gallery-welcome")]
    public class Gallery
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxWords(10)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxWords(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxWords(10)]
        public string ImageTitle { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;
    }
}