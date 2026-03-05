using System.ComponentModel.DataAnnotations;

namespace SPSUL.Models.Display.TeacherModels
{
    public class TeacherBase
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u024F\u1E00-\u1EFF ]+$", ErrorMessage = "Jméno může obsahovat pouze písmena.")]
        public required string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u024F\u1E00-\u1EFF ]+$", ErrorMessage = "Příjmení může obsahovat pouze písmena.")]
        public required string LastName { get; set; }

        [Required]
        public required string NickName { get; set; }
        public List<int>? TitleIds { get; set; }
        public List<int>? RoleIds { get; set; }

    }
}
