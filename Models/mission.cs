using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace yogloansdotnet.Models
{
    [Table("mission")]
    [Keyless]   // <— IMPORTANT
    public class mission
    {
        public string header { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
    }
}
