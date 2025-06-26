using System.ComponentModel.DataAnnotations;

namespace yogloansdotnet.Models
{
    public class PdfAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var filePath = value.ToString();
            if (string.IsNullOrEmpty(filePath))
                return ValidationResult.Success;

            if (!filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return new ValidationResult("The file must be a PDF document.");
            }

            return ValidationResult.Success;
        }
    }
} 