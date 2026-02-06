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

        // Students Forms
        [HttpGet]
        [Route("/api/Config/StudentEditForm/{id}")]
        public async Task<IActionResult> StudentEditForm(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Neplatné ID");
            }

            Student? student = await _ctx.Students.Include(e => e.ClassesStudents)
                .ThenInclude(e => e.Classes)
                .FirstOrDefaultAsync(e => e.StudentId == id);

            if (student == null)
            {
                return NotFound("Třída nebyla nalezena.");
            }

            StudentFormEditVM vm = new()
            {
                Student = student,
                Classes = await _ctx.Classes.ToListAsync(),
            };

            return PartialView("Views/Shared/Config/StudentEditForm.cshtml", vm);
        }

        [HttpGet]
        [Route("/api/Config/StudentCreateForm/")]
        public async Task<IActionResult> StudentCreateForm()
        {
            StudentFormCreateVM vm = new()
            {
                Classes = await _ctx.Classes.ToListAsync(),
            };

            return PartialView("Views/Shared/Config/StudentCreateForm.cshtml", vm);
        }

        // ============================================
        // PROFILE UPDATE (current logged-in teacher)
        // ============================================
        [HttpPut]
        [Route("/api/config/profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateDto dto)
        {
            int? teacherId = HttpContext.Session.GetInt32("TeacherId");
            if (teacherId == null)
                return Unauthorized("Nejste přihlášen.");

            var teacher = await _ctx.Teachers.FindAsync(teacherId.Value);
            if (teacher == null)
                return NotFound("Učitel nebyl nalezen.");

            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                return BadRequest("Jméno a příjmení jsou povinné.");

            if (string.IsNullOrWhiteSpace(dto.NickName))
                return BadRequest("Přezdívka je povinná.");

            teacher.FirstName = dto.FirstName.Trim();
            teacher.LastName = dto.LastName.Trim();
            teacher.NickName = dto.NickName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                if (dto.NewPassword.Length < 4)
                    return BadRequest("Heslo musí mít alespoň 4 znaky.");

                teacher.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            }

            await _ctx.SaveChangesAsync();
            return Ok("Profil byl úspěšně aktualizován.");
        }
    }

    public class ProfileUpdateDto
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string NickName { get; set; } = "";
        public string? NewPassword { get; set; }
    }
}
