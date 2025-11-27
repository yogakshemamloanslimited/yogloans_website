using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yogloansdotnet.Models
{
    [Table("Announcementswelcome")]
    public class AnnouncementsWelcomeModel
    {
        [Key]

        public string title { get; set; } = string.Empty;
        public byte[]? image { get; set; }    // not required
        public byte[]? image2 { get; set; }   // not required

     
    
    }
}
