using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Bussiness;
using SPSUL.Models.Data;
namespace SPSUL.Controllers.API
{
    public class TeacherController : Controller
    {
        private readonly SpsulContext _ctx;
        public TeacherController(SpsulContext ctx)
        {
            _ctx = ctx;
        }

        [HttpPost]
        [Route("api/[controller]")]
        public async Task<IActionResult> AddTeacher(TeacherModel model)
        {
            if (ModelState.IsValid)
            {
                Teacher teacher = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    NickName = model.NickName,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    RoleId = model.RoleId,
                };

                await _ctx.AddAsync(teacher);
                await _ctx.SaveChangesAsync();
            }

            return RedirectToAction();
        }

        [HttpGet]
        public async Task<IActionResult> Login(string Name, string Password)
        {
            try
            {
                string hash = _ctx.Teachers.Where(e => e.NickName == Name).FirstOrDefault().Password;
                bool isValid = BCrypt.Net.BCrypt.Verify(Password, hash);

                if (isValid)
                {
                    return RedirectToAction("Student Panel");
                }
                else
                {
                    return PartialView();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
