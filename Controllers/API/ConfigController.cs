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
        private readonly IWebHostEnvironment _env;
        public ConfigController(SpsulContext ctx, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _ctx = ctx;
            _env = env;
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
            if (id <= 0)
            {
                return BadRequest("Neplatné ID");
            }
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

        // Classes Forms
        [HttpGet]
        [Route("/api/Config/ClassesEditForm/{id}")]
        public async Task<IActionResult> ClassesEditForm(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Neplatné ID");
            }

            Classes? myClasses = await _ctx.Classes.Include(e => e.ClassesFields).FirstOrDefaultAsync(e => e.ClassesId == id);

            if (myClasses == null) { 
                return NotFound("Třída nebyla nalezena.");
            }

            ClassesFormEditVM vm = new()
            {
                Classes = myClasses,
                StudentFields = await _ctx.StudentFields.ToListAsync(),
            };

            return PartialView("Views/Shared/Config/ClassesEditForm.cshtml", vm);
        }

        [HttpGet]
        [Route("/api/Config/ClassesCreateForm/")]
        public async Task<IActionResult> ClassesCreateForm()
        {
            ClassesFormCreateVM vm = new()
            {
                StudentFields = await _ctx.StudentFields.ToListAsync(),
            };

            return PartialView("Views/Shared/Config/ClassesCreateForm.cshtml", vm);
        }

        [HttpGet]
        public IActionResult DownloadDocumentation()
        {
            string filePath = Path.Combine(_env.WebRootPath, "docs", "technicka-dokumentace.pdf");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Soubor nebyl nalezen.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", "Elektronicka Dokumentace - SPSUL Tvoření Testu.pdf");
        }
    }
}
