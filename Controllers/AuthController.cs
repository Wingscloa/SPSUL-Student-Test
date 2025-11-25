using Microsoft.AspNetCore.Mvc;
using SPSUL.Models.Bussiness;
using SPSUL.Models;
using SPSUL.Models.Data;
using System.Security.Cryptography;
using BCrypt.Net;

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
    }
}
