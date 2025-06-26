using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yogloansdotnet.Models
{
    [Table("Offer")] // Optional: maps to this table name in DB
    public class OfferModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Loan { get; set; } = string.Empty;

        [Required]
        public string OfferHeader { get; set; } = string.Empty;

        [Required]
        public string OfferContent { get; set; } = string.Empty;

    
    }
}
