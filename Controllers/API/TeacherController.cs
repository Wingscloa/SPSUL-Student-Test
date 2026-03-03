using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display;
using SPSUL.Models.Display.TeacherModels;
using Microsoft.EntityFrameworkCore;

namespace SPSUL.Controllers.API
{
    /// <summary>
    /// REST API pro CRUD operace nad učiteli v konfiguračním modálu.
    ///
    /// Endpointy:
    ///   GET  /api/teacher/row         – stránkovaný seznam učitelů (vrátí HTML partial)
    ///   GET  /api/teacher/content/    – filtrovaný obsah tabulky
    ///   POST /api/teacher             – vytvoří nového učitele
    ///   PUT  /api/teacher             – upravuje existujícího učitele
    ///   POST /api/teacher/delete      – bulk mazání učitelů dle ID
    ///
    /// Zabezpečení:
    ///   [AutoValidateAntiforgeryToken] – chrání před CSRF útoky
    ///   [RequirePermission(CrudTeachers)] – jen učitelé s rolí Administrátor nebo Učitelátor
    ///   Všechny CUD operace píší do AuditLog.
    /// </summary>
    [AutoValidateAntiforgeryToken]
    public class TeacherController : Controller
    {
        private readonly SpsulContext _ctx;
        private readonly ILogger<TeacherController> _logger;
        private readonly AuditService _audit;

        public TeacherController(SpsulContext ctx, ILogger<TeacherController> logger, AuditService audit)
        {
            _ctx = ctx;
            _logger = logger;
            _audit = audit;
        }

        [Route("api/[controller]/row")]
        [HttpGet]
        public async Task<IActionResult> GetRow(int id)
        {
            try
            {
                List<Teacher>? teachers = await _ctx.Teachers
                    .Where(t => t.TeacherId == id)
                    .OrderBy(t => t.TeacherId)
                    .ToListAsync();

                if(teachers == null || teachers.Count == 0)
                {
                    return NotFound("Nebyl nalezen záznam učitele.");
                }

                return PartialView("_TeacherRows", teachers);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání počtu záznamů učitelů.");
                return StatusCode(500, "Došlo k chybě při získávání počtu záznamů učitelů.");
            }
        }

        [Route("api/[controller]/content")]
        [HttpPost]
        public async Task<IActionResult> GetTableContent([FromBody] TeacherFilter model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    List<Teacher> query = await _ctx.Teachers.Where(e => 
                        (model.SearchFilter != null ? 
                            (e.FirstName.Contains(model.SearchFilter) || 
                             e.LastName.Contains(model.SearchFilter) || 
                             e.NickName.Contains(model.SearchFilter)) 
                            : true) &&
                        (model.ActiveFilter.HasValue ? e.IsActive == model.ActiveFilter.Value : true)
                        ).Include(e => e.Titles).ThenInclude(e => e.Title).Include(e => e.TeacherRoles)
                        .ToListAsync();


                    if(model.TitleFilterIds != null && model.TitleFilterIds.Count > 0)
                    {
                        query = query.Where(e => e.Titles.Any(t => model.TitleFilterIds.Contains(t.TitleId))).ToList();
                    }

                    if(model.RoleFilterIds != null && model.RoleFilterIds.Count > 0)
                    {
                        query = query.Where(e => e.TeacherRoles.Any(r => model.RoleFilterIds.Contains(r.RoleId))).ToList();
                    }

                    List<Teacher>? rows = query
                        .Skip((model.PageNumber - 1) * model.PageSize)
                        .Take(model.PageSize)
                        .OrderByDescending(e => e.TeacherId)
                        .ToList();

                    int count = query.Count;

                    PaginatedList<Teacher> paginatedList = new(rows, count, model.PageNumber, model.PageSize);

                    return PartialView("TableContent/_TableContent", paginatedList);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Chyba při získávání počtu záznamů učitelů.");
                    return StatusCode(500, "Došlo k chybě při získávání počtu záznamů učitelů.");
                }
            }
            else
            {
                _logger.LogError("Neplatný model dat pro filtrování učitelů.");
                return BadRequest("Neplatný model dat pro filtrování učitelů.");
            }
        }

        [Route("api/[controller]")]
        [HttpPost]
        [RequirePermission(AppPermissions.CrudTeachers, AppPermissions.All)]
        public async Task<IActionResult> Post([FromBody] TeacherCreate model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    Teacher teacher = new()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        NickName = model.NickName,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                        IsActive = true
                    };

                    await _ctx.Teachers.AddAsync(teacher);
                    await _ctx.SaveChangesAsync();

                    List<TeacherTitle> teacherTitles = model.TitleIds.Select(titleId => new TeacherTitle
                    {
                        TeacherId = teacher.TeacherId,
                        TitleId = titleId
                    }).ToList();

                    List<TeacherRole> teacherRoles = model.RoleIds.Select(roleid => new TeacherRole
                    {
                        TeacherId = teacher.TeacherId,
                        RoleId = roleid

                    }).ToList();

                    await _ctx.AddRangeAsync(teacherTitles);
                    await _ctx.AddRangeAsync(teacherRoles);
                    await _ctx.SaveChangesAsync();
                    await _audit.LogAsync("Vytvořen", "Učitel", teacher.TeacherId.ToString(), $"{teacher.FirstName} {teacher.LastName}");
                    return Ok("Vytvoření učitele proběhlo v pořádku");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Chyba při vytváření záznamu učitele.");
                    return StatusCode(500, "Došlo k chybě při vytváření záznamu učitele.");
                }
            }
            else
            {
                return BadRequest("Neplatný model dat pro vytvoření učitele.");
            }

        }

        [HttpPut]
        [Route("api/[controller]")]
        [RequirePermission(AppPermissions.CrudTeachers, AppPermissions.All)]
        public async Task<IActionResult> Put([FromBody] TeacherUpdate model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    Teacher? teacher = await _ctx.Teachers.FindAsync(model.TeacherId);
                    if(teacher == null)
                    {
                        return NotFound("Učitel s daným ID nebyl nalezen.");
                    }

                    List<TeacherRole>? roles = await _ctx.TeacherRoles.Where(tr => tr.TeacherId == model.TeacherId).ToListAsync();
                    List<TeacherRole>? teacherRoles = model.RoleIds != null ? model.RoleIds.Select(roleid => new TeacherRole
                    {
                        TeacherId = model.TeacherId,
                        RoleId = roleid
                    }).ToList() : null;

                    _ctx.RemoveRange(roles); 
                    if (teacherRoles != null) 
                    {
                        _ctx.AddRange(teacherRoles); 
                    } 

                    List<TeacherTitle>? titles = await _ctx.TeacherTitles.Where(tt => tt.TeacherId == model.TeacherId).ToListAsync();
                    List<TeacherTitle>? teacherTitles = model.TitleIds != null ? model.TitleIds.Select(titleId => new TeacherTitle
                    {
                        TeacherId = model.TeacherId,
                        TitleId = titleId
                    }).ToList() : null;

                    _ctx.RemoveRange(titles); 
                    if (teacherTitles != null) 
                    { 
                        _ctx.AddRange(teacherTitles); 
                    }; 

                    teacher.FirstName = model.FirstName;
                    teacher.LastName = model.LastName;
                    teacher.NickName = model.NickName;
                    teacher.IsActive = model.IsActive;

                    await _ctx.SaveChangesAsync();

                    await _audit.LogAsync("Upraven", "Učitel", model.TeacherId.ToString(), $"{model.FirstName} {model.LastName}");
                    return Ok("Aktualizace učitele proběhla v pořádku");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Chyba při aktualizaci záznamu učitele s ID: {TeacherId}", model.TeacherId);
                    return StatusCode(500, "Došlo k chybě při aktualizaci záznamu učitele.");
                }
            }
            else
            {
                return BadRequest("Neplatný model dat pro aktualizaci učitele.");
            }
        }

        [HttpPost]
        [Route("api/[controller]/delete")]
        [RequirePermission(AppPermissions.All)]
        public async Task<IActionResult> Delete([FromBody] List<int>? Ids)
        {
            try
            {
                 if (Ids == null || Ids.Count == 0)
                {
                    return BadRequest("Nebyla vybrána žádna ID");
                }

                List<Teacher> teachers = await _ctx.Teachers.Where(t => Ids.Contains(t.TeacherId)).ToListAsync();

                if (teachers.Count == 0)
                {
                    return NotFound("Nebyla nalezena žádna data pro zadaná ID");
                }

                _ctx.Teachers.RemoveRange(teachers);
                await _ctx.SaveChangesAsync();
                await _audit.LogAsync("Smazán", "Učitel", string.Join(",", Ids), $"{teachers.Count} učitelů");
                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Chyba při mazání záznamů učitelů s ID: {Ids}", string.Join(", ", Ids));
                return StatusCode(500, "Došlo k chybě při mazání záznamů učitelů.");
            }
        }
    }
}
