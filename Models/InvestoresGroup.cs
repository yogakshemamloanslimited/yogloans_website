using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yogloansdotnet.Models
{
    [Table("Investor")]
    public class InvestoresGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Profile { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

           [Required]
        public string Role { get; set; } = string.Empty;

         [Required]
        public string Phone { get; set; } = string.Empty;

        
         [Required]
        public string Mobile { get; set; } = string.Empty;

          
         [Required]
        public string Address { get; set; } = string.Empty;
    }
} 