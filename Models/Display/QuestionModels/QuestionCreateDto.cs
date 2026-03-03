using System.ComponentModel.DataAnnotations;
using SPSUL.Controllers.Attribute;

namespace SPSUL.Models.Display.QuestionModels
{
    public class QuestionCreateDto
    {
        [Required(ErrorMessage = "Nadpis je povinný.")]
        [StringLength(128, ErrorMessage = "Nadpis nesmí být delší než 128 znaků.")]
        [MinLength(3, ErrorMessage = "Nadpis musí mít alespoň 3 znaky.")]
        public string Header { get; set; } = string.Empty;

        [Required(ErrorMessage = "Popis je povinný.")]
        [StringLength(512, ErrorMessage = "Popis nesmí být delší než 512 znaků.")]
        [MinLength(10, ErrorMessage = "Popis musí mít alespoň 10 znaků.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Typ otázky je povinný.")]
        [Range(1, int.MaxValue, ErrorMessage = "Musíte vybrat platný typ otázky.")]
        public int QuestionTypeId { get; set; }

        [Required(ErrorMessage = "Předmět je povinný.")]
        [Range(1, int.MaxValue, ErrorMessage = "Musíte vybrat platný předmět.")]
        public int FieldId { get; set; }

        [Required(ErrorMessage = "Možnosti odpovědí jsou povinné.")]
        [MinLength(2, ErrorMessage = "Otázka musí mít alespoň 2 možnosti odpovědí.")]
        [MaxLength(10, ErrorMessage = "Otázka může mít maximálně 10 možností odpovědí.")]
        [OneCorrectOption(ErrorMessage = "Alespoň jedna odpověď musí být označena jako správná.")]
        public required List<QuestionOptionDto> Options { get; set; }
    }
}
