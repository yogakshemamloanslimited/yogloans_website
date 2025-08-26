using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yogloansdotnet.Attributes; //


namespace yogloansdotnet.Models
{
 public class GalleryViewModel
{
    public List<Gallery> WelcomeGallery { get; set; }
    public List<GalleryImagesModel> OtherGallery { get; set; }
}

}