using System.ComponentModel.DataAnnotations;
using SPSUL.Controllers.Attribute;

namespace SPSUL.Models.Display.QuestionModels
{
    public class QuestionOptionDto
    {
        [StringLength(256, ErrorMessage = "Text odpovědi nesmí být delší než 256 znaků.")]
        public string Text { get; set; } = string.Empty;

        [Base64Image(ErrorMessage = "Neplatný formát obrázku. Obrázek musí být ve formátu Base64.")]
        public string? ImageBase64 { get; set; }

        [Required(ErrorMessage = "Musíte určit, zda je odpověď správná.")]
        public bool IsCorrect { get; set; }
    }
}
