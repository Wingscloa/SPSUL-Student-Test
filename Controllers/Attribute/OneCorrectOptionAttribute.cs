using System.ComponentModel.DataAnnotations;
using SPSUL.Models.Display.QuestionModels;

namespace SPSUL.Controllers.Attribute
{
    /// <summary>
    /// Validace, že seznam možností obsahuje alespoň jednu správnou odpověď.
    ///
    /// Proč:
    ///   Otázka bez správné odpovědi by nefungovala při vyhodnocení testu.
    ///   Tato validace to zachytí na serveru ještě před uložením do DB.
    ///
    /// Použití:
    ///   [OneCorrectOption(ErrorMessage = "Označte alespoň jednu správnou odpověď.")]
    ///   public List&lt;QuestionOptionDto&gt; Options { get; set; }
    /// </summary>
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
