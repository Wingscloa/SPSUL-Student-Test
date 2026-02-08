using System.ComponentModel.DataAnnotations;


namespace SPSUL.Controllers.Attribute
{
    public class Base64ImageAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string base64String = value.ToString()!;

            if (string.IsNullOrWhiteSpace(base64String))
            {
                return ValidationResult.Success;
            }

            try
            {
                // Odstranění data URI prefixu pokud existuje
                if (base64String.Contains(','))
                {
                    var header = base64String.Split(',')[0];
                    if (!header.Contains("data:image/"))
                    {
                        return new ValidationResult("Base64 string musí obsahovat platný data URI prefix pro obrázek (např. 'data:image/png;base64,').");
                    }

                    var base64Data = base64String.Split(',')[1];
                    Convert.FromBase64String(base64Data);
                }
                else
                {
                    Convert.FromBase64String(base64String);
                }

                return ValidationResult.Success;
            }
            catch (FormatException)
            {
                return new ValidationResult(ErrorMessage ?? "Neplatný formát Base64.");
            }
        }
    }
}