using System.ComponentModel.DataAnnotations;

namespace SPSUL.Models.Display.TestModels
{
    public class TestCreateDto
    {
        [Required(ErrorMessage = "Název testu je povinný.")]
        [StringLength(128, ErrorMessage = "Název nesmí být delší než 128 znaků.")]
        [MinLength(3, ErrorMessage = "Název musí mít alespoň 3 znaky.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Předmět je povinný.")]
        [Range(1, int.MaxValue, ErrorMessage = "Musíte vybrat platný předmět.")]
        public int StudentFieldId { get; set; }

        [Required(ErrorMessage = "Otázky jsou povinné.")]
        [MinLength(1, ErrorMessage = "Test musí obsahovat alespoň 1 otázku.")]
        public List<int> QuestionIds { get; set; } = new();

        [Range(1, 180, ErrorMessage = "Časový limit musí být mezi 1 a 180 minutami.")]
        public int? TimeLimit { get; set; }

        public string? Description { get; set; }
    }
}
