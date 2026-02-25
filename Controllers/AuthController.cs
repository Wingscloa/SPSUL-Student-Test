using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display.Auth;

namespace SPSUL.Controllers
{
    public class AuthController : Controller
    {
        private readonly SpsulContext _ctx;
        private readonly IMemoryCache _cache;
        public AuthController(SpsulContext ctx, IMemoryCache cache)
        {
            _ctx = ctx;
            _cache = cache;
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    Teacher? teacher = _ctx.Teachers.FirstOrDefault(e => e.NickName == model.NickName);

                    if (teacher != null && BCrypt.Net.BCrypt.Verify(model.Password, teacher.PasswordHash))
                    {
                        HttpContext.Session.SetInt32("TeacherId", teacher.TeacherId);
                        HttpContext.Session.SetString("Name", teacher.NickName);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        this.Alert("Zkontroluj přihlašovací údaje.", NotificationType.Error);
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            else
            {
                return View();
            }
        }
        public IActionResult Logout()
        {
            var x = HttpContext.Session.GetInt32("TeacherId");
            _cache.Remove($"TeacherName");
            HttpContext.Session.Clear(); 
            return RedirectToAction("Login");
        }
    }
}