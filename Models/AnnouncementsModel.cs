using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yogloansdotnet.Models
{
    [Table("Announcements")]
    public class AnnouncementsModel
    {
        [Key]
        public int Id { get; set; } 

        public string Title { get; set; } = string.Empty;

       
        public byte[]? FilePath { get; set; } 
    }
} 