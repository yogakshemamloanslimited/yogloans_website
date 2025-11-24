using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace yogloansdotnet.Models
{
    [Table("vission")]
    [Keyless]
    public class vission
    {
        public string? header { get; set; }
        public string? content { get; set; }
    }

}
