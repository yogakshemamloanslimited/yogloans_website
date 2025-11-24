using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yogloansdotnet.Attributes; // Include your custom attribute

namespace yogloansdotnet.Models
{
    [Table("announcements_create")]
    public class announcements_create
    {
        [Key]

        public int Id { get; set; }
        public string? title { get; set; }

        public byte[]? image { get; set; }

        public string? content { get; set; }
    }


}
