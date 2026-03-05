using System.ComponentModel.DataAnnotations;

namespace SPSUL.Models.Display.StudentModels
{
    public class StudentCreate
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u024F\u1E00-\u1EFF ]+$", ErrorMessage = "Jméno může obsahovat pouze písmena.")]
        public required string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u024F\u1E00-\u1EFF ]+$", ErrorMessage = "Příjmení může obsahovat pouze písmena.")]
        public required string LastName { get; set; }

        public List<int>? ClassesIds { get; set; }
    }
}
