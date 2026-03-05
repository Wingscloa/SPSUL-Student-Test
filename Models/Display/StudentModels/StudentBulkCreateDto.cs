using System.ComponentModel.DataAnnotations;

namespace SPSUL.Models.Display.StudentModels
{
    public class StudentBulkCreateDto
    {
        [Required(ErrorMessage = "Musíte zadat alespoň jednoho studenta.")]
        [MinLength(1, ErrorMessage = "Musíte zadat alespoň jednoho studenta.")]
        public List<StudentBulkItem> Students { get; set; } = [];

        public List<int>? ClassesIds { get; set; }
    }

    public class StudentBulkItem
    {
        [Required(ErrorMessage = "Jméno je povinné.")]
        [StringLength(64, MinimumLength = 2, ErrorMessage = "Jméno musí mít 2–64 znaků.")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u024F\u1E00-\u1EFF ]+$", ErrorMessage = "Jméno může obsahovat pouze písmena.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Příjmení je povinné.")]
        [StringLength(64, MinimumLength = 2, ErrorMessage = "Příjmení musí mít 2–64 znaků.")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u024F\u1E00-\u1EFF ]+$", ErrorMessage = "Příjmení může obsahovat pouze písmena.")]
        public string LastName { get; set; } = string.Empty;
    }
}
