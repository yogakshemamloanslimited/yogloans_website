using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yogloansdotnet.Attributes;

namespace yogloansdotnet.Models
{
    [Table("Career")]
    public class CareerModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxWords(10)]
        public string Job { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? File { get; set; }

        [MaxWords(10)]
        public string Role { get; set; } = string.Empty;

        [Required]
        [MaxWords(10)]
        public string Department { get; set; } = string.Empty;

        [MaxWords(10)]
        public string? Salary_range_to { get; set; }

        [MaxWords(10)]
        public string? Salary_range_from { get; set; }

         [MaxWords(10)]
        public string? Experience_from { get; set; }

        [MaxWords(10)]
        public string? Experience_to { get; set; }

        [Required]
        [MaxWords(10)]
        public string Shift { get; set; } = string.Empty;

        [Required]
        [MaxWords(10)]
        public string de_name { get; set; } = string.Empty;

        [MaxWords(200)]
        public string? Discription { get; set; }

        [MaxWords(10)]
        public string? Status { get; set; }
    }
}
