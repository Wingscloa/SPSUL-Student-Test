using Microsoft.AspNetCore.Mvc;
using SPSUL.Models.Data;
using SPSUL.Models;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models.Display.ConfigModels;

namespace SPSUL.Controllers.API
{
    public class ConfigController : Controller
    {
        private readonly SpsulContext _ctx;
        public ConfigController(SpsulContext ctx, IHttpContextAccessor httpContextAccessor)
        {
            _ctx = ctx;
        }

        [HttpGet("/api/config/section/{sectionName}")]
        public IActionResult LoadSection(string sectionName)
        {
            return ViewComponent(sectionName);
        }


        // TEACHER FORMS
        [HttpGet]
        [Route("/api/Config/TeacherEditForm/{id}")]
        public async Task<IActionResult> TeacherEditForm(int id)
        {
            TeacherFormEditVM vm = new()
            {
                Teacher = await _ctx.Teachers.Include(e => e.TeacherRoles).Include(e => e.Titles).FirstOrDefaultAsync(e => e.TeacherId == id),
                Titles = await _ctx.Titles.ToListAsync(),
                Roles = await _ctx.Roles.ToListAsync()
            };

            return PartialView("Views/Shared/Config/TeacherEditForm.cshtml", vm);
        }

        [HttpGet]
        [Route("/api/Config/TeacherCreateForm/")]
        public async Task<IActionResult> TeacherCreateForm()
        {
            TeacherFormCreateVM vm = new()
            {
                Titles = await _ctx.Titles.ToListAsync(),
                Roles = await _ctx.Roles.ToListAsync()
            };

            return PartialView("Views/Shared/Config/TeacherCreateForm.cshtml", vm);
        }

        // STUDENT
    }
}
