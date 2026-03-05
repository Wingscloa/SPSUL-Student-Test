using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display;
using SPSUL.Models.Display.StudentModels;
using Microsoft.EntityFrameworkCore;

namespace SPSUL.Controllers.API
{
    /// <summary>
    /// REST API pro CRUD operace nad studenty, včetně aktivace/deaktivace a hromadného vytvoření.
    ///
    /// Endpointy:
    ///   GET  /api/student/content/         – filtrovaný seznam studentů (HTML partial)
    ///   POST /api/student                  – vytvoří studenta
    ///   PUT  /api/student                  – upravuje studenta
    ///   POST /api/student/delete           – bulk mazání studentů dle ID
    ///   POST /Students/Activate/{id}       – aktivuje studenta
    ///   POST /Students/Deactivate/{id}     – deaktivuje studenta
    ///   POST /api/student/bulk             – hromadné vytvoření studentů (CSV-like vstup)
    ///
    /// Zabezpečení:
    ///   [AutoValidateAntiforgeryToken] – chrání před CSRF útoky
    ///   [RequirePermission(CrudStudents)] – jen Administrátor, Tvůrce nebo Studentátor
    ///   Všechny CUD operace píší do AuditLog.
    /// </summary>
    [AutoValidateAntiforgeryToken]
    public class StudentController : Controller
    {
        private readonly SpsulContext _ctx;
        private readonly ILogger<StudentController> _logger;
        private readonly AuditService _audit;

        public StudentController(SpsulContext ctx, ILogger<StudentController> logger, AuditService audit)
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
                List<Student>? students = await _ctx.Students
                    .Where(t => t.StudentId == id)
                    .ToListAsync();

                if (students == null || students.Count == 0)
                {
                    return NotFound("Nebyl nalezen záznam studenta.");
                }

                return PartialView("_StudentRows", students);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání počtu záznamů studentů.");
                return StatusCode(500, "Došlo k chybě při získávání počtu záznamů studentů.");
            }
        }

        [Route("api/[controller]/content")]
        [HttpPost]
        public async Task<IActionResult> GetTableContent([FromBody] StudentFilter model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<Student> query = await _ctx.Students.Where(e =>
                        (model.SearchFilter != null ?
                            (e.FirstName.Contains(model.SearchFilter) ||
                             e.LastName.Contains(model.SearchFilter))
                            : true) &&
                        (model.ActiveFilter.HasValue ? e.IsActive == model.ActiveFilter.Value : true)
                        ).Include(e => e.ClassesStudents).ThenInclude(e => e.Classes).Include(e => e.StudentTests)
                        .ThenInclude(e => e.Test)
                        .ToListAsync();

                    if (model.ClassFilterIds != null && model.ClassFilterIds.Count > 0)
                    {
                        query = query.Where(e => e.ClassesStudents.Any(t => model.ClassFilterIds.Contains(t.ClassesId))).ToList();
                    }

                    if (model.TestFilterIds != null && model.TestFilterIds.Count > 0)
                    {
                        query = query.Where(e => e.StudentTests.Any(t => model.TestFilterIds.Contains(t.TestId))).ToList();
                    }

                    List<Student>? rows = query
                        .Skip((model.PageNumber - 1) * model.PageSize)
                        .Take(model.PageSize)
                        .OrderByDescending(e => e.StudentId)
                        .ToList();

                    int count = query.Count;

                    PaginatedList<Student> paginatedList = new(rows, count, model.PageNumber, model.PageSize);

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
        [RequirePermission(AppPermissions.CrudStudents, AppPermissions.ManageStudents, AppPermissions.All)]
        public async Task<IActionResult> Post([FromBody] StudentCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Student student = new()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        IsActive = true
                    };

                    await _ctx.Students.AddAsync(student);
                    await _ctx.SaveChangesAsync();

                    if (model.ClassesIds != null)
                    {
                        List<ClassesStudent> classesStudents = model.ClassesIds.Select(t => new ClassesStudent
                        {
                            StudentId = student.StudentId,
                            ClassesId = t,
                        }).ToList();
                        await _ctx.AddRangeAsync(classesStudents);
                    await _ctx.SaveChangesAsync();
                    }
                   
                    await _audit.LogAsync("Vytvořen", "Student", student.StudentId.ToString(), $"{student.FirstName} {student.LastName}");
                    return Ok("Vytvoření studenta proběhlo v pořádku");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Chyba při vytváření záznamu studenta.");
                    return StatusCode(500, "Došlo k chybě při vytváření záznamu studenta.");
                }
            }
            else
            {
                return BadRequest("Neplatný model dat pro vytvoření studenta.");
            }

        }

        [HttpPut]
        [Route("api/[controller]")]
        [RequirePermission(AppPermissions.CrudStudents, AppPermissions.ManageStudents, AppPermissions.All)]
        public async Task<IActionResult> Put([FromBody] StudentUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Student? student = await _ctx.Students.FindAsync(model.StudentId);
                    if (student == null)
                    {
                        return NotFound("Student s daným ID nebyl nalezen.");
                    }

                    student.FirstName = model.FirstName;
                    student.LastName = model.LastName;
                    student.IsActive = model.IsActive;


                    List<ClassesStudent>? classes = await _ctx.ClassesStudents.Where(t => t.StudentId == model.StudentId).ToListAsync();
                    _ctx.RemoveRange(classes);

                    if (model.ClassesIds != null && model.ClassesIds.Count > 0)
                    {
                        List<ClassesStudent> newClasses = model.ClassesIds.Select(e => new ClassesStudent
                        {
                            ClassesId = e,
                            StudentId = model.StudentId,
                        }).ToList();
                        _ctx.AddRange(newClasses);
                    }

                    await _ctx.SaveChangesAsync();

                    await _audit.LogAsync("Upraven", "Student", model.StudentId.ToString(), $"{model.FirstName} {model.LastName}");
                    return Ok("Aktualizace studenta proběhla v pořádku");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Chyba při aktualizaci záznamu učitele s ID: {StudentId}", model.StudentId);
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
        [RequirePermission(AppPermissions.CrudStudents, AppPermissions.ManageStudents, AppPermissions.All)]
        public async Task<IActionResult> Delete([FromBody] List<int>? Ids)
        {
            try
            {
                if (Ids == null || Ids.Count == 0)
                {
                    return BadRequest("Nebyla vybrána žádna ID");
                }

                List<Student> students = await _ctx.Students.Where(t => Ids.Contains(t.StudentId)).ToListAsync();

                if (students.Count == 0)
                {
                    return NotFound("Nebyla nalezena žádna data pro zadaná ID");
                }

                _ctx.Students.RemoveRange(students);
                await _ctx.SaveChangesAsync();
                await _audit.LogAsync("Smazán", "Student", string.Join(",", Ids), $"{students.Count} studentů");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při mazání záznamů studentů s ID: {Ids}", string.Join(", ", Ids != null ? Ids : ""));
                return StatusCode(500, "Došlo k chybě při mazání záznamů studentů.");
            }
        }
    }
}
