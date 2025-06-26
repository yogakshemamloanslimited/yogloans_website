using System.ComponentModel.DataAnnotations;

namespace yogloansdotnet.Models
{
    public class LoginFormModel
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }

        public string? ErrorMessage { get; set; }
    }
} 