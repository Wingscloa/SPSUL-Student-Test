using System.ComponentModel.DataAnnotations;
using SPSUL.Models.Display.QuestionModels;

namespace SPSUL.Controllers.Attribute
{
    public class OneCorrectOptionAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is List<QuestionOptionDto> options)
            {
                if (options.Any(o => o.IsCorrect))
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult(ErrorMessage ?? "Alespoň jedna odpověď musí být označena jako správná.");
            }
            return new ValidationResult("Neplatný formát možností odpovědí.");
        }
    }
}
