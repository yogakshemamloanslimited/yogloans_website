using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using yogloansdotnet.Attributes;

namespace yogloansdotnet.Models
{
    [Table("Annual-report")]
    public class AnnualreportModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxWords(10)]
        public string title { get; set; } = string.Empty;

        [Required]
        public string pdf { get; set; } = string.Empty;
    }

    
}
