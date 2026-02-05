using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Display.TestModels;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models.Data;
using System.Text.Json;

namespace SPSUL.Controllers
{
    [LoginRequired]
    public class TestController : Controller
    {

        private readonly SpsulContext _ctx;
        public TestController(SpsulContext context)
        {
            _ctx = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Test(string TestId)
        {
            if (!ModelState.IsValid)
            {
                this.Alert("Neplatný vstup identifičního čísla testu.", NotificationType.Error);
                return View("Index");
            }
            else
            {
                try
                {
                    bool testExists = await _ctx.StudentTests.AnyAsync(e => e.LoginId == TestId);

                    if(!testExists)
                    {
                        this.Alert("Zadané identifiční číslo testu neexistuje.", NotificationType.Error);
                        return Ok();
                    }

                    return View("Test",TestId);
                }
                catch (Exception ex)
                {
                    return View("Error");
                }
            }
        }
        public IActionResult Example()
        {
            return View();
        }
        public async Task<IActionResult> Create()
        {
            try
            {
                CreateViewModel model = new()
                {
                   Classes = await _ctx.Classes.Include(e => e.ClassesStudents).ToListAsync(),
                   Questions = await _ctx.Questions
                       .Include(e => e.QuestionType)
                       .Include(e => e.QuestionOptions)
                       .Where(e => e.IsActive == true)
                       .ToListAsync(),
                   Fields = await _ctx.StudentFields.Where(e => e.IsActive == true).ToListAsync(),
                   Students = await _ctx.Students.ToListAsync(),
                   Types = await _ctx.QuestionTypes.Where(e => e.IsActive == true).ToListAsync()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTest([FromBody] TestCreateDto dto)
        {
            // Validace pomocí ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new 
                { 
                    message = "Neplatná vstupní data.",
                    errors
                });
            }

            using var transaction = await _ctx.Database.BeginTransactionAsync();

            try
            {
                // Načtení otázek pro vytvoření snapshotu
                var questions = await _ctx.Questions
                    .Include(q => q.QuestionOptions)
                    .Include(q => q.QuestionType)
                    .Where(q => dto.QuestionIds.Contains(q.QuestionId))
                    .ToListAsync();

                if (questions.Count != dto.QuestionIds.Count)
                {
                    return BadRequest(new { message = "Některé vybrané otázky neexistují." });
                }

                // Vytvoření snapshotu otázek (JSON)
                var questionSnapshot = questions.Select(q => new
                {
                    q.QuestionId,
                    q.Header,
                    q.Description,
                    QuestionType = q.QuestionType.Name,
                    Options = q.QuestionOptions.Select(o => new
                    {
                        o.Text,
                        o.ImageKey,
                        o.IsCorrect
                    }).ToList()
                }).ToList();

                string snapshotJson = JsonSerializer.Serialize(questionSnapshot);

                // Vytvoření testu
                var test = new Test
                {
                    Name = dto.Name.Trim(),
                    CreatorId = (int)HttpContext.Items["CurrentUserId"],
                    StudentFieldId = dto.StudentFieldId,
                    QuestionSnapshot = snapshotJson,
                    IsActive = true
                };

                _ctx.Tests.Add(test);
                await _ctx.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new 
                { 
                    message = "Test byl úspěšně vytvořen!",
                    testId = test.TestId,
                    questionsCount = questions.Count
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new 
                { 
                    message = "Chyba při vytváření testu.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message 
                });
            }
        }
    }
}
