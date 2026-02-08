using System.ComponentModel.DataAnnotations;

namespace SPSUL.Models.Display.StudentModels
{
    public class StudentBulkCreateDto
    {
        [Required(ErrorMessage = "Musíte zadat alespoò jednoho studenta.")]
        [MinLength(1, ErrorMessage = "Musíte zadat alespoò jednoho studenta.")]
        public List<StudentBulkItem> Students { get; set; } = [];

        public List<int>? ClassesIds { get; set; }
    }

    public class StudentBulkItem
    {
        [Required(ErrorMessage = "Jméno je povinné.")]
        [StringLength(64, MinimumLength = 2, ErrorMessage = "Jméno musí mít 2–64 znakù.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pøíjmení je povinné.")]
        [StringLength(64, MinimumLength = 2, ErrorMessage = "Pøíjmení musí mít 2–64 znakù.")]
        public string LastName { get; set; } = string.Empty;
    }
}
