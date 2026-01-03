using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display.Auth;

namespace SPSUL.Controllers
{
    public class AuthController : Controller
    {
        private readonly SpsulContext _ctx;
        public AuthController(SpsulContext ctx)
        {
            _ctx = ctx;
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
        public IActionResult Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                    Teacher teacher = new()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        NickName = model.NickName,
                        PasswordHash = passwordHash,
                    };

                    _ctx.Add(teacher);
                    _ctx.SaveChanges();
                    return RedirectToAction("Login");
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

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    Teacher teacher = _ctx.Teachers.FirstOrDefault(e => e.NickName == model.NickName);

                    if (teacher != null && BCrypt.Net.BCrypt.Verify(model.Password, teacher.PasswordHash))
                    {
                        HttpContext.Session.SetInt32("TeacherId", teacher.TeacherId);
                        HttpContext.Session.SetString("Name", teacher.NickName);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
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
            HttpContext.Session.Clear(); 
            return RedirectToAction("Login");
        }
    }
}