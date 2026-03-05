using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display.ClassesModels;

namespace SPSUL.Controllers
{
    [LoginRequired]
    public class ClassesPageController : Controller
    {
        private readonly SpsulContext _ctx;
        private readonly LookupCacheService _lookup;

        public ClassesPageController(SpsulContext ctx, LookupCacheService lookup)
        {
            _ctx = ctx;
            _lookup = lookup;
        }

        public async Task<IActionResult> Index(string? name, int? fieldId, bool? active)
        {
            var query = _ctx.Classes
                .AsNoTracking()
                .Include(c => c.ClassesFields).ThenInclude(cf => cf.StudentField)
                .Include(c => c.ClassesStudents).ThenInclude(cs => cs.Student)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(c => c.Name.Contains(name));

            if (active.HasValue)
                query = query.Where(c => c.IsActive == active.Value);

            if (fieldId.HasValue)
                query = query.Where(c => c.ClassesFields.Any(cf => cf.StudentFieldId == fieldId.Value));

            var classes = await query
                .OrderByDescending(c => c.IsActive)
                .ThenByDescending(c => c.ClassesId)
                .ToListAsync();

            var students = await _ctx.Students
                .AsNoTracking()
                .Where(s => s.IsActive)
                .Include(s => s.ClassesStudents)
                .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
                .ToListAsync();

            var model = new ClassesIndexVM
            {
                Classes = classes,
                Fields = await _lookup.GetActiveFieldsAsync(),
                Students = students,
                Name = name,
                FieldId = fieldId,
                Active = active
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignStudents([FromBody] AssignStudentsDto dto)
        {
            if (dto.ClassId <= 0 || dto.StudentIds == null || dto.StudentIds.Count == 0)
                return BadRequest(new { message = "Neplatná data." });

            var cls = await _ctx.Classes.Include(c => c.ClassesStudents).FirstOrDefaultAsync(c => c.ClassesId == dto.ClassId);
            if (cls == null)
                return NotFound(new { message = "Třída nebyla nalezena." });

            var existingIds = cls.ClassesStudents.Select(cs => cs.StudentId).ToHashSet();
            var newLinks = dto.StudentIds
                .Where(sid => !existingIds.Contains(sid))
                .Select(sid => new ClassesStudent { ClassesId = dto.ClassId, StudentId = sid })
                .ToList();

            if (newLinks.Count == 0)
                return Ok(new { message = "Všichni vybraní studenti jsou již přiřazeni.", added = 0 });

            _ctx.ClassesStudents.AddRange(newLinks);
            await _ctx.SaveChangesAsync();

            return Ok(new { message = $"{newLinks.Count} studentů přiřazeno do třídy.", added = newLinks.Count });
        }

        [HttpPost]
        public async Task<IActionResult> UnassignStudents([FromBody] AssignStudentsDto dto)
        {
            if (dto.ClassId <= 0 || dto.StudentIds == null || dto.StudentIds.Count == 0)
                return BadRequest(new { message = "Neplatná data." });

            var links = await _ctx.ClassesStudents
                .Where(cs => cs.ClassesId == dto.ClassId && dto.StudentIds.Contains(cs.StudentId))
                .ToListAsync();

            if (links.Count == 0)
                return Ok(new { message = "Žádní studenti k odebrání.", removed = 0 });

            _ctx.ClassesStudents.RemoveRange(links);
            await _ctx.SaveChangesAsync();

            return Ok(new { message = $"{links.Count} studentů odebráno z třídy.", removed = links.Count });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive([FromBody] int id)
        {
            var cls = await _ctx.Classes.FindAsync(id);
            if (cls == null)
                return NotFound(new { message = "Třída nebyla nalezena." });

            cls.IsActive = !cls.IsActive;
            await _ctx.SaveChangesAsync();

            var status = cls.IsActive ? "aktivována" : "deaktivována";
            return Ok(new { message = $"Třída byla {status}.", isActive = cls.IsActive });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            var classes = await _ctx.Classes
                .Include(c => c.ClassesFields)
                .Include(c => c.ClassesStudents)
                .Where(c => ids.Contains(c.ClassesId))
                .ToListAsync();

            if (classes.Count == 0)
                return NotFound(new { message = "Žádné třídy nebyly nalezeny." });

            _ctx.ClassesFields.RemoveRange(classes.SelectMany(c => c.ClassesFields));
            _ctx.ClassesStudents.RemoveRange(classes.SelectMany(c => c.ClassesStudents));
            _ctx.Classes.RemoveRange(classes);
            await _ctx.SaveChangesAsync();

            return Ok(new { message = $"{classes.Count} tříd smazáno." });
        }
    }
}
