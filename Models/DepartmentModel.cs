using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yogloansdotnet.Attributes; // Include your custom attribute

namespace yogloansdotnet.Models
{
    [Table("Department")]
    public class DepartmentModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxWords(10)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxWords(200)]
        public string Discription { get; set; } = string.Empty;
    }
}
