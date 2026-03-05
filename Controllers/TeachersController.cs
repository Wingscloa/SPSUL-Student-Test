using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display.TeacherModels;

namespace SPSUL.Controllers
{
    [LoginRequired]
    public class TeachersController : Controller
    {
        private readonly SpsulContext _ctx;
        private readonly LookupCacheService _lookup;

        public TeachersController(SpsulContext ctx, LookupCacheService lookup)
        {
            _ctx = ctx;
            _lookup = lookup;
        }

        public async Task<IActionResult> Index(string? name, int? roleId, int? titleId, bool? active)
        {
            var query = _ctx.Teachers
                .AsNoTracking()
                .Include(t => t.Titles).ThenInclude(tt => tt.Title)
                .Include(t => t.TeacherRoles).ThenInclude(tr => tr.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(t => t.FirstName.Contains(name) || t.LastName.Contains(name) || t.NickName.Contains(name));

            if (active.HasValue)
                query = query.Where(t => t.IsActive == active.Value);

            if (roleId.HasValue)
                query = query.Where(t => t.TeacherRoles.Any(tr => tr.RoleId == roleId.Value));

            if (titleId.HasValue)
                query = query.Where(t => t.Titles.Any(tt => tt.TitleId == titleId.Value));

            var teachers = await query
                .OrderByDescending(t => t.IsActive)
                .ThenByDescending(t => t.TeacherId)
                .ToListAsync();

            var model = new TeacherIndexVM
            {
                Teachers = teachers,
                Roles = await _lookup.GetRolesAsync(),
                Titles = await _lookup.GetTitlesAsync(),
                Name = name,
                RoleId = roleId,
                TitleId = titleId,
                Active = active
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive([FromBody] int id)
        {
            var teacher = await _ctx.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound(new { message = "Učitel nebyl nalezen." });

            teacher.IsActive = !teacher.IsActive;
            await _ctx.SaveChangesAsync();

            var status = teacher.IsActive ? "aktivován" : "deaktivován";
            return Ok(new { message = $"Učitel byl {status}.", isActive = teacher.IsActive });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            var teachers = await _ctx.Teachers
                .Include(t => t.Titles)
                .Include(t => t.TeacherRoles)
                .Where(t => ids.Contains(t.TeacherId))
                .ToListAsync();

            if (teachers.Count == 0)
                return NotFound(new { message = "Žádní učitelé nebyli nalezeni." });

            _ctx.TeacherTitles.RemoveRange(teachers.SelectMany(t => t.Titles));
            _ctx.TeacherRoles.RemoveRange(teachers.SelectMany(t => t.TeacherRoles));
            _ctx.Teachers.RemoveRange(teachers);
            await _ctx.SaveChangesAsync();

            return Ok(new { message = $"{teachers.Count} učitelů smazáno." });
        }
    }
}
