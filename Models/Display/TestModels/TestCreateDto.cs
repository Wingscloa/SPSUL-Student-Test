using System.ComponentModel.DataAnnotations;

namespace SPSUL.Models.Display.TestModels
{
    public class TestCreateDto
    {
        [Required(ErrorMessage = "Název testu je povinný.")]
        [StringLength(128, ErrorMessage = "Název nesmí být delší než 128 znakù.")]
        [MinLength(3, ErrorMessage = "Název musí mít alespoò 3 znaky.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pøedmìt je povinný.")]
        [Range(1, int.MaxValue, ErrorMessage = "Musíte vybrat platný pøedmìt.")]
        public int StudentFieldId { get; set; }

        [Required(ErrorMessage = "Otázky jsou povinné.")]
        [MinLength(1, ErrorMessage = "Test musí obsahovat alespoò 1 otázku.")]
        public List<int> QuestionIds { get; set; } = new();

        [Range(1, 180, ErrorMessage = "Èasový limit musí být mezi 1 a 180 minutami.")]
        public int? TimeLimit { get; set; }

        public string? Description { get; set; }
    }
}
