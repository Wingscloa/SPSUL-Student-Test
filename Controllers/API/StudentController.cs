using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display;
using SPSUL.Models.Display.StudentModels;
using Microsoft.EntityFrameworkCore;

namespace SPSUL.Controllers.API
{
    public class StudentController : Controller
    {
        private readonly SpsulContext _ctx;
        private readonly ILogger<StudentController> _logger;

        public StudentController(SpsulContext ctx, ILogger<StudentController> logger)
        {
            _ctx = ctx;
            _logger = logger;
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
                    List<ClassesStudent>? newClasses = model.ClassesIds.Select(e => new ClassesStudent
                    {
                        ClassesId = e,
                        StudentId = model.StudentId,
                    }).ToList();

                    _ctx.RemoveRange(classes);
                    if (newClasses != null)
                    {
                        _ctx.AddRange(newClasses);
                    };

                    await _ctx.SaveChangesAsync();

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
