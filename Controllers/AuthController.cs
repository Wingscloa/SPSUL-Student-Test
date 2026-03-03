using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display.Auth;

namespace SPSUL.Controllers
{
    /// <summary>
    /// Správa přihlášení a odhlášení učitelů.
    ///
    /// Endpointy:
    ///   GET  /Auth/Login   – zobrazí přihlašovací formulář
    ///   POST /Auth/Login   – zpracuje přihlášení (BCrypt hash ověření)
    ///   GET  /Auth/Logout  – smaže session a přesměruje na Login
    ///
    /// Zabezpečení:
    ///   - Hesla jsou hashovana BCrypt algoritmem (nikdy plain text).
    ///   - Login POST je chráněn [RateLimit] – max 5 pokusů za 5 minut z jedné IP.
    ///   - Úspěšné přihlášení se zaznamená do AuditLog.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly SpsulContext _ctx;
        private readonly IMemoryCache _cache;
        private readonly AuditService _audit;
        public AuthController(SpsulContext ctx, IMemoryCache cache, AuditService audit)
        {
            _ctx = ctx;
            _cache = cache;
            _audit = audit;
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
        [RateLimit(MaxAttempts = 5, WindowSeconds = 300)]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    Teacher? teacher = await _ctx.Teachers.FirstOrDefaultAsync(e => e.NickName == model.NickName);

                    if (teacher != null && BCrypt.Net.BCrypt.Verify(model.Password, teacher.PasswordHash))
                    {
                        HttpContext.Session.SetInt32("TeacherId", teacher.TeacherId);
                        HttpContext.Session.SetString("Name", teacher.NickName);
                        await _audit.LogAsync("Přihlášení", "Učitel", teacher.TeacherId.ToString(), teacher.NickName);
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
            var teacherId = HttpContext.Session.GetInt32("TeacherId");
            if (teacherId.HasValue)
            {
                _cache.Remove($"teacher:{teacherId}:TeacherName");
            }
            HttpContext.Session.Clear(); 
            return RedirectToAction("Login");
        }
    }
}