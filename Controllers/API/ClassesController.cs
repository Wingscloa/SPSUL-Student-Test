using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display;
using SPSUL.Models.Display.ClassesModels;
using Microsoft.EntityFrameworkCore;

namespace SPSUL.Controllers.API
{
    /// <summary>
    /// REST API pro CRUD operace nad třídami v konfiguračním modálu.
    ///
    /// Endpointy:
    ///   GET  /api/classes/content/   – filtrovaný seznam tříd (HTML partial)
    ///   POST /api/classes            – vytvoří třídu
    ///   PUT  /api/classes            – upravuje třídu
    ///   POST /api/classes/delete     – bulk mazání tříd dle ID
    ///
    /// Zabezpečení:
    ///   [AutoValidateAntiforgeryToken] – chrání před CSRF útoky
    ///   [RequirePermission(ManageClasses)] – jen Administrátor nebo Tvůrce
    ///   Všechny CUD operace píší do AuditLog.
    /// </summary>
    [AutoValidateAntiforgeryToken]
    public class ClassesController : Controller
    {
        private readonly SpsulContext _ctx;
        private readonly ILogger<ClassesController> _logger;
        private readonly AuditService _audit;

        public ClassesController(SpsulContext ctx, ILogger<ClassesController> logger, AuditService audit)
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
                List<Classes>? classes = await _ctx.Classes
                    .Where(c => c.ClassesId > id)
                    .OrderBy(c => c.ClassesId)
                    .ToListAsync();

                if (classes == null || classes.Count == 0)
                {
                    return NotFound("Nebyl nalezen záznam třídy.");
                }

                return PartialView("_ClassesRows", classes);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání počtu záznamů tříd.");
                return StatusCode(500, "Došlo k chybě při získávání počtu záznamů tříd.");
            }
        }

        [Route("api/[controller]/content")]
        [HttpPost]
        public async Task<IActionResult> GetTableContent([FromBody] ClassesFilter model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<Classes> query = await _ctx.Classes.Where(e =>
                        (model.SearchFilter != null ?
                        (e.Name.Contains(model.SearchFilter)) : true) &&
                        (model.ActiveFilter.HasValue ? e.IsActive == model.ActiveFilter.Value : true) &&
                        (model.StartFromFilter.HasValue ? e.StartFrom == model.StartFromFilter.Value : true) &&
                        (model.EndToFilter.HasValue ? e.EndTo == model.EndToFilter.Value : true))
                        .Include(e => e.ClassesFields).ThenInclude(e => e.StudentField)
                        .ToListAsync();

                    if (model.FieldFilterIds != null && model.FieldFilterIds.Count > 0)
                    {
                        query = query.Where(e => e.ClassesFields.Any(t => model.FieldFilterIds.Contains(t.StudentFieldId))).ToList();
                    }


                    List<Classes>? rows = query
                        .Skip((model.PageNumber - 1) * model.PageSize)
                        .Take(model.PageSize)
                        .OrderByDescending(e => e.ClassesId)
                        .ToList();

                    int count = query.Count;

                    PaginatedList<Classes> paginatedList = new(rows, count, model.PageNumber, model.PageSize);

                    return PartialView("TableContent/_TableContent", paginatedList);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Chyba při získávání počtu záznamů tříd.");
                    return StatusCode(500, "Došlo k chybě při získávání počtu záznamů tříd.");
                }
            }
            else
            {
                _logger.LogError("Neplatný model dat pro filtrování tříd.");
                return BadRequest("Neplatný model dat pro filtrování tříd.");
            }
        }

        [Route("api/[controller]")]
        [HttpPost]
        [RequirePermission(AppPermissions.ManageClasses, AppPermissions.All)]
        public async Task<IActionResult> Post([FromBody] ClassesCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Classes newClass = new()
                    {
                        Name = model.Name,
                        IsActive = true,
                        StartFrom = model.StartFrom,
                        EndTo = model.EndTo
                    };

                    await _ctx.Classes.AddAsync(newClass);
                    await _ctx.SaveChangesAsync();

                    List<ClassesFields>? classFields = model.StudentFieldIds.Select(fieldId => new ClassesFields
                    {
                        ClassesId = newClass.ClassesId,
                        StudentFieldId = fieldId
                    }).ToList();

                    await _ctx.ClassesFields.AddRangeAsync(classFields);
                    await _ctx.SaveChangesAsync();

                    await _audit.LogAsync("Vytvořena", "Třída", newClass.ClassesId.ToString(), newClass.Name);
                    return Ok("Vytvoření třídy proběhlo v pořádku");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Chyba při vytváření záznamu třídy.");
                    return StatusCode(500, "Došlo k chybě při vytváření záznamu třídy.");
                }
            }
            else
            {
                return BadRequest("Neplatný model dat pro vytvoření třídy.");
            }

        }

        [HttpPut]
        [Route("api/[controller]")]
        [RequirePermission(AppPermissions.ManageClasses, AppPermissions.All)]
        public async Task<IActionResult> Put([FromBody] ClassesUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Classes? existingClass = await _ctx.Classes
                        .Include(c => c.ClassesFields)
                        .FirstOrDefaultAsync(c => c.ClassesId == model.ClassesId);

                    if (existingClass == null) { return NotFound("Záznam třídy nebyl nalezen.");}
                    
                    existingClass.Name = model.Name;
                    existingClass.StartFrom = model.StartFrom;
                    existingClass.EndTo = model.EndTo;
                    existingClass.IsActive = model.IsActive;

                    List<ClassesFields> fields = await _ctx.ClassesFields.Where(e => e.ClassesId == model.ClassesId).ToListAsync();

                    _ctx.RemoveRange(fields);
                    List<ClassesFields>? updatedFields = model.StudentFieldIds.Select(fieldId => new ClassesFields
                    {
                        ClassesId = existingClass.ClassesId,
                        StudentFieldId = fieldId
                    }).ToList();

                    await _ctx.ClassesFields.AddRangeAsync(updatedFields);
                    await _ctx.SaveChangesAsync();
                    await _audit.LogAsync("Upravena", "Třída", model.ClassesId.ToString(), model.Name);
                    return Ok("Aktualizace třídy proběhla v pořádku");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Chyba při aktualizaci záznamu třídy s ID: {ClassesId}", model.ClassesId);
                    return StatusCode(500, "Došlo k chybě při aktualizaci záznamu třídy.");
                }
            }
            else
            {
                return BadRequest("Neplatný model dat pro aktualizaci třídy.");
            }
        }

        [HttpPost]
        [Route("api/[controller]/delete")]
        [RequirePermission(AppPermissions.ManageClasses, AppPermissions.All)]
        public async Task<IActionResult> Delete([FromBody] List<int>? Ids)
        {
            try
            {
                if (Ids == null || Ids.Count == 0)
                {
                    return BadRequest("Nebyla vybrána žádna ID");
                }

                List<Classes>? classes = await _ctx.Classes.Where(c => Ids.Contains(c.ClassesId)).ToListAsync();

                if (classes.Count == 0)
                {
                    return NotFound("Nebyla nalezena žádna data pro zadaná ID");
                }

                _ctx.Classes.RemoveRange(classes);
                await _ctx.SaveChangesAsync();
                await _audit.LogAsync("Smazána", "Třída", string.Join(",", Ids), $"{classes.Count} tříd");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při mazání záznamů učitelů s ID: {Ids}", string.Join(", ", Ids));
                return StatusCode(500, "Došlo k chybě při mazání záznamů třídy.");
            }
        }
    }
}
