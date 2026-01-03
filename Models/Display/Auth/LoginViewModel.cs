using System.ComponentModel.DataAnnotations;

namespace SPSUL.Models.Display.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Přezdívka je povinná")]
        public required string NickName { get; set; }
        [Required(ErrorMessage = "Heslo je povinné")]
        public required string Password { get; set; }
    }
}
