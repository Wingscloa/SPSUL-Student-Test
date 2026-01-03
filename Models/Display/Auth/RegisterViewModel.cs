using System.ComponentModel.DataAnnotations;

namespace SPSUL.Models.Display.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage="Jméno je povinné")]
        public required string FirstName { get; set; }
        [Required(ErrorMessage = "Příjmení je povinné")]
        public required string LastName { get; set; }
        [Required(ErrorMessage = "Přezdívka je povinná")]
        public required string NickName { get; set; }
        [Required(ErrorMessage = "Heslo je povinné")]
        public required string Password { get; set; }
    }
}
