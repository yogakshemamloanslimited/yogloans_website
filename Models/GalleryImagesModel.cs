using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yogloansdotnet.Attributes; //

namespace yogloansdotnet.Models
{
    [Table("GalleryImages")]
    public class GalleryImagesModel
    {
        [Key]
        public int Id { get; set; }

      

        [Required]
        [MaxWords(10)]
        public string ImageTitle { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

    }
    }
