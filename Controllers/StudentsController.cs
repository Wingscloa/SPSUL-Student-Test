using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models.Data;
using SPSUL.Models;
using SPSUL.Models.Display.StudentModels;

namespace SPSUL.Controllers
{
    /// <summary>
    /// Zobrazení seznamů studentů pro učitele (read-only view).
    ///
    /// Poznámka:
    ///   Tento controller slouží pouze k ZOBRAZENí (Index, filtrace).
    ///   Skutečné CRUD operace (vytvoření, úprava, mazání, aktivace) jsou v
    ///   Controllers/API/StudentController.cs a volá je frontend přes AJAX.
    /// </summary>
    [LoginRequired]
    public class StudentsController : Controller
    {
        private readonly SpsulContext _ctx;
        private readonly LookupCacheService _lookup;

        public StudentsController(SpsulContext ctx, LookupCacheService lookup)
        {
            _ctx = ctx;
            _lookup = lookup;
        }

        public async Task<IActionResult> Index(string? name, int? classId, int? fieldId, bool? active)
        {
            var query = _ctx.Students
                .AsNoTracking()
                .Include(e => e.ClassesStudents).ThenInclude(e => e.Classes)
                    .ThenInclude(c => c.ClassesFields).ThenInclude(cf => cf.StudentField)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name));

            if (active.HasValue)
                query = query.Where(e => e.IsActive == active.Value);

            if (classId.HasValue)
                query = query.Where(e => e.ClassesStudents.Any(cs => cs.ClassesId == classId.Value));

            if (fieldId.HasValue)
                query = query.Where(e => e.ClassesStudents.Any(cs =>
                    cs.Classes.ClassesFields.Any(cf => cf.StudentFieldId == fieldId.Value)));

            var students = await query
                .OrderByDescending(e => e.IsActive)
                .ThenByDescending(e => e.StudentId)
                .ToListAsync();

            var model = new StudentIndexVM
            {
                Students = students,
                Classes = await _lookup.GetActiveClassesAsync(),
                Fields = await _lookup.GetActiveFieldsAsync(),
                Name = name,
                ClassId = classId,
                FieldId = fieldId,
                Active = active
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> BulkCreate([FromBody] StudentBulkCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(e => e.Value!.Errors.Select(err => err.ErrorMessage))
                    .ToList();
                return BadRequest(new { message = "Neplatná data.", errors });
            }

            var newStudents = dto.Students.Select(s => new Student
            {
                FirstName = s.FirstName.Trim(),
                LastName = s.LastName.Trim(),
                IsActive = true
            }).ToList();

            _ctx.Students.AddRange(newStudents);
            await _ctx.SaveChangesAsync();

            if (dto.ClassesIds != null && dto.ClassesIds.Count > 0)
            {
                var classLinks = newStudents
                    .SelectMany(s => dto.ClassesIds.Select(cId => new ClassesStudent
                    {
                        StudentId = s.StudentId,
                        ClassesId = cId
                    }))
                    .ToList();

                _ctx.ClassesStudents.AddRange(classLinks);
                await _ctx.SaveChangesAsync();
            }

            return Ok(new { message = $"Úspěšně vytvořeno {newStudents.Count} studentů." });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive([FromBody] int id)
        {
            var student = await _ctx.Students.FindAsync(id);
            if (student == null)
                return NotFound(new { message = "Student nebyl nalezen." });

            student.IsActive = !student.IsActive;
            await _ctx.SaveChangesAsync();

            var status = student.IsActive ? "aktivován" : "deaktivován";
            return Ok(new { message = $"Student byl {status}.", isActive = student.IsActive });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            var students = await _ctx.Students
                .Include(s => s.ClassesStudents)
                .Where(s => ids.Contains(s.StudentId))
                .ToListAsync();

            if (students.Count == 0)
                return NotFound(new { message = "Žádní studenti nebyli nalezeni." });

            _ctx.ClassesStudents.RemoveRange(students.SelectMany(s => s.ClassesStudents));
            _ctx.Students.RemoveRange(students);
            await _ctx.SaveChangesAsync();

            return Ok(new { message = $"{students.Count} studentů smazáno." });
        }
    }
}
