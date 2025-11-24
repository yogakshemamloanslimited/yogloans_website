using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yogloansdotnet.Models;

namespace yogloansdotnet.Models
{
    [NotMapped]
    public class AnnouncementData
    {
        public List<announcements_create> Create { get; set; } = new();

        public AnnouncementsWelcomeModel Welcome { get; set; }
    }
}

