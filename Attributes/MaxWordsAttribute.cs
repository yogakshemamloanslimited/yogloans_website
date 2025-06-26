using System.ComponentModel.DataAnnotations;

namespace yogloansdotnet.Attributes
{
    public class MaxWords : ValidationAttribute
    {
        private readonly int _maxWords;

        public MaxWords(int maxWords)
        {
            _maxWords = maxWords;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var text = value.ToString();
            if (string.IsNullOrWhiteSpace(text))
                return ValidationResult.Success;

            var wordCount = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

            if (wordCount > _maxWords)
            {
                return new ValidationResult($"The {validationContext.DisplayName} cannot exceed {_maxWords} words.");
            }

            return ValidationResult.Success;
        }
    }
} 