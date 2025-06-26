using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yogloansdotnet.Attributes; // Include your custom attribute

namespace yogloansdotnet.Models
{
    [Table("AboutContent")]
    public class AboutContentModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxWords(10)]
        public string Header { get; set; } = string.Empty;

        [Required]
        [MaxWords(200)]
        public string Content { get; set; } = string.Empty;
    }
}
