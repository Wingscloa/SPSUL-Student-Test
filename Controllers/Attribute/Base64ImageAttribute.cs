using System.ComponentModel.DataAnnotations;


namespace SPSUL.Controllers.Attribute
{
    public class Base64ImageAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Nullable, takže null je OK
            }

            string base64String = value.ToString()!;

            try
            {
                // Odstranění data URI prefixu pokud existuje
                var base64Data = base64String.Contains(',')
                    ? base64String.Split(',')[1]
                    : base64String;

                // Pokus o dekódování
                Convert.FromBase64String(base64Data);

                // Kontrola, zda obsahuje data URI prefix pro obrázek
                if (base64String.Contains(','))
                {
                    var header = base64String.Split(',')[0];
                    if (!header.Contains("data:image/"))
                    {
                        return new ValidationResult("Base64 string musí obsahovat platný data URI prefix pro obrázek (např. 'data:image/png;base64,').");
                    }
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