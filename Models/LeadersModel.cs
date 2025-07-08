using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yogloansdotnet.Attributes; // Include your custom attribute

namespace yogloansdotnet.Models
{
    [Table("Leaders")]
    public class LeadersModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxWords(10)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxWords(10)]
        public string Post { get; set; } = string.Empty;

        [Required]
        [MaxWords(500)]
        public string Profile { get; set; } = string.Empty;

       

        [Required]
        [MaxWords(500)]
        public string About { get; set; } = string.Empty;
    }
}
