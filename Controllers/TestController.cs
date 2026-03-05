using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Display.TestModels;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models.Data;
using System.Text.Json;

namespace SPSUL.Controllers
{
    /// <summary>
    /// Správa testů ze strany učitele.
    ///
    /// Funkce:
    ///   Index      – seznam všech testů s filtrem a rychlými statistikami
    ///   Create     – tvorba nového testu (výběr otázek ze snapshotu)
    ///   Edit       – úprava existujícího testu
    ///   Assignments – přehled přiřazení studentů ke konkrétnímu testu + tisk kódů
    ///   Example    – demo test (dostupný bez přihlášení, [AllowAnonymousTest])
    ///   Take       – stránka pro studenta při psaní testu (přes LoginId)
    ///
    /// Klíčový koncept – QuestionSnapshot:
    ///   Při uložení testu se otázky zkopírují jako JSON snapshot.
    ///   Díky tomu pozdější editace otázek neovlivní již vytvořené testy.
    /// </summary>
    [LoginRequired]
    public class TestController : Controller
    {
        private readonly SpsulContext _ctx;
        private readonly PdfService _pdf;
        private readonly LookupCacheService _lookup;
        public TestController(SpsulContext context, PdfService pdf, LookupCacheService lookup)
        {
            _ctx = context;
            _pdf = pdf;
            _lookup = lookup;
        }

        // ============================================
        // EXAMPLE - demo test without login
        // ============================================
        [HttpGet]
        [AllowAnonymousTest]
        public IActionResult Example()
        {
            return View();
        }

        // ============================================
        // INDEX
        // ============================================
        public async Task<IActionResult> Index()
        {
            var tests = await _ctx.Tests
                .AsNoTracking()
                .Include(t => t.Creator)
                .Include(t => t.StudentField)
                .Include(t => t.StudentTests)
                .OrderByDescending(t => t.IsActive)
                .ThenByDescending(t => t.TestId)
                .ToListAsync();

            var students = await _ctx.Students
                .AsNoTracking()
                .Include(s => s.ClassesStudents)
                    .ThenInclude(cs => cs.Classes)
                .Where(s => s.IsActive)
                .ToListAsync();

            var classes = await _lookup.GetActiveClassesAsync();

            ViewBag.Students = students;
            ViewBag.Classes = classes;

            return View(tests);
        }

        // ============================================
        // ASSIGNMENTS - historie přiřazení pro test
        // ============================================
        public async Task<IActionResult> Assignments(int id)
        {
            var test = await _ctx.Tests
                .Include(t => t.Creator)
                .Include(t => t.StudentField)
                .FirstOrDefaultAsync(t => t.TestId == id);

            if (test == null)
                return RedirectToAction("Index");

            var assignments = await _ctx.StudentTests
                .Include(st => st.Student)
                .Where(st => st.TestId == id)
                .OrderByDescending(st => st.StartedAt)
                .ToListAsync();

            ViewBag.Test = test;
            ViewBag.Assignments = assignments;

            return View();
        }

        // ============================================
        // DOWNLOAD CODES PDF
        // ============================================
        public async Task<IActionResult> DownloadCodesPdf(int id)
        {
            var test = await _ctx.Tests.FindAsync(id);
            if (test == null) return RedirectToAction("Index");

            var assignments = await _ctx.StudentTests
                .Include(st => st.Student)
                .Where(st => st.TestId == id)
                .ToListAsync();

            var pdfBytes = _pdf.GenerateCodesPdf(test, assignments);
            return File(pdfBytes, "application/pdf", $"Kody-{test.Name}.pdf");
        }

        // ============================================
        // DOWNLOAD CODES PDF - selected only
        // ============================================
        [HttpPost]
        public async Task<IActionResult> DownloadSelectedCodesPdf([FromBody] SelectedCodesDto dto)
        {
            var test = await _ctx.Tests.FindAsync(dto.TestId);
            if (test == null) return NotFound();

            var assignments = await _ctx.StudentTests
                .Include(st => st.Student)
                .Where(st => st.TestId == dto.TestId && dto.LoginIds.Contains(st.LoginId))
                .ToListAsync();

            if (assignments.Count == 0)
                return BadRequest(new { message = "Žádné kódy nebyly vybrány." });

            var pdfBytes = _pdf.GenerateCodesPdf(test, assignments);
            return File(pdfBytes, "application/pdf", $"Kody-{test.Name}.pdf");
        }

        // ============================================
        // ASSIGN
        // ============================================
        [HttpPost]
        public async Task<IActionResult> Assign([FromBody] AssignTestDto dto)
        {
            if (dto.StudentIds == null || dto.StudentIds.Count == 0)
                return BadRequest(new { message = "Musíte vybrat alespoň jednoho studenta." });

            var test = await _ctx.Tests.FindAsync(dto.TestId);
            if (test == null)
                return NotFound(new { message = "Test nebyl nalezen." });

            if (!test.IsActive)
                return BadRequest(new { message = "Tento test není aktivní." });

            var existingAssignments = await _ctx.StudentTests
                .Where(st => st.TestId == dto.TestId)
                .Select(st => st.StudentId)
                .ToListAsync();

            var newStudentIds = dto.StudentIds.Except(existingAssignments).ToList();
            if (newStudentIds.Count == 0)
                return BadRequest(new { message = "Všichni vybraní studenti už mají tento test přiřazený." });

            var assignments = new List<StudentTest>();
            var loginIds = new List<object>();

            foreach (var studentId in newStudentIds)
            {
                var loginId = GenerateLoginId();

                var assignment = new StudentTest
                {
                    StudentId = studentId,
                    TestId = dto.TestId,
                    LoginId = loginId,
                    StartedAt = DateTime.MinValue,
                    FinishedAt = DateTime.MinValue,
                    ResultSnapshot = "{}"
                };

                assignments.Add(assignment);
                var student = await _ctx.Students.FindAsync(studentId);
                loginIds.Add(new
                {
                    studentId,
                    studentName = student != null ? $"{student.FirstName} {student.LastName}" : "?",
                    loginId
                });
            }

            _ctx.StudentTests.AddRange(assignments);
            await _ctx.SaveChangesAsync();

            return Ok(new
            {
                message = $"Test přiřazen {newStudentIds.Count} studentům.",
                assignments = loginIds
            });
        }

        // ============================================
        // REASSIGN - opakování testu
        // ============================================
        [HttpPost]
        public async Task<IActionResult> Reassign([FromBody] ReassignDto dto)
        {
            var old = await _ctx.StudentTests
                .FirstOrDefaultAsync(st => st.StudentId == dto.StudentId && st.TestId == dto.TestId);

            if (old == null)
                return NotFound(new { message = "Přiřazení nebylo nalezeno." });

            // Smazat staré přiřazení
            _ctx.StudentTests.Remove(old);
            await _ctx.SaveChangesAsync();

            // Vytvořit nové s novým LoginId
            var loginId = GenerateLoginId();
            var newAssignment = new StudentTest
            {
                StudentId = dto.StudentId,
                TestId = dto.TestId,
                LoginId = loginId,
                StartedAt = DateTime.MinValue,
                FinishedAt = DateTime.MinValue,
                ResultSnapshot = "{}"
            };

            _ctx.StudentTests.Add(newAssignment);
            await _ctx.SaveChangesAsync();

            return Ok(new { message = "Test byl znovu přiřazen.", loginId });
        }

        // ============================================
        // TAKE - student vstupuje do testu
        // ============================================
        [HttpGet]
        [AllowAnonymousTest]
        public async Task<IActionResult> Take(string testId)
        {
            if (string.IsNullOrWhiteSpace(testId))
            {
                TempData["TestError"] = "Zadejte číslo testu.";
                return RedirectToAction("Test", "Auth");
            }

            var assignment = await _ctx.StudentTests
                .Include(st => st.Test)
                .Include(st => st.Student)
                .FirstOrDefaultAsync(st => st.LoginId == testId);

            if (assignment == null)
            {
                TempData["TestError"] = "Zadané číslo testu neexistuje.";
                return RedirectToAction("Test", "Auth");
            }

            if (!assignment.Test.IsActive)
            {
                TempData["TestError"] = "Tento test není momentálně aktivní. Kontaktujte svého učitele.";
                return RedirectToAction("Test", "Auth");
            }

            if (assignment.FinishedAt != DateTime.MinValue)
            {
                TempData["TestError"] = "Tento test byl již dokončen.";
                return RedirectToAction("Test", "Auth");
            }

            // Deserializovat otázky
            var questions = JsonSerializer.Deserialize<List<QuestionSnapshotItem>>(
                assignment.Test.QuestionSnapshot) ?? [];

            // Pokud student začíná poprvé
            if (assignment.StartedAt == DateTime.MinValue)
            {
                assignment.StartedAt = DateTime.Now;

                // Shuffle otázky + odpovědi pokud test to vyžaduje
                if (assignment.Test.ShuffleQuestions)
                {
                    var rng = new Random();
                    questions = questions.OrderBy(_ => rng.Next()).ToList();
                    foreach (var q in questions)
                        q.Options = q.Options.OrderBy(_ => rng.Next()).ToList();

                    // Uložit pořadí pro konzistentní zobrazení při návratu
                    var order = questions.Select(q => q.QuestionId).ToList();
                    assignment.ShuffleOrder = JsonSerializer.Serialize(order);
                }

                var resultSnapshot = new TestResultSnapshot
                {
                    Answers = questions.Select(q => new AnswerSnapshot
                    {
                        QuestionId = q.QuestionId,
                        SelectedOptions = new List<string>()
                    }).ToList()
                };

                assignment.ResultSnapshot = JsonSerializer.Serialize(resultSnapshot);
                await _ctx.SaveChangesAsync();
            }
            else if (!string.IsNullOrEmpty(assignment.ShuffleOrder))
            {
                // Pokračování - obnovit uložené pořadí
                var order = JsonSerializer.Deserialize<List<int>>(assignment.ShuffleOrder) ?? [];
                if (order.Count > 0)
                {
                    var dict = questions.ToDictionary(q => q.QuestionId);
                    questions = order.Where(id => dict.ContainsKey(id)).Select(id => dict[id]).ToList();
                }
            }

            // Kontrola časového limitu
            if (assignment.Test.TimeLimitMinutes.HasValue)
            {
                var elapsed = DateTime.Now - assignment.StartedAt;
                if (elapsed.TotalMinutes >= assignment.Test.TimeLimitMinutes.Value)
                {
                    // Čas vypršel - automaticky dokončit
                    if (assignment.FinishedAt == DateTime.MinValue)
                    {
                        assignment.FinishedAt = DateTime.Now;
                        await _ctx.SaveChangesAsync();
                    }
                    TempData["TestError"] = "Časový limit vypršel. Test byl automaticky odevzdán.";
                    return RedirectToAction("Test", "Auth");
                }
            }

            var existingResult = JsonSerializer.Deserialize<TestResultSnapshot>(
                assignment.ResultSnapshot) ?? new TestResultSnapshot();

            var model = new TakeTestViewModel
            {
                LoginId = assignment.LoginId,
                TestName = assignment.Test.Name,
                StudentName = $"{assignment.Student.FirstName} {assignment.Student.LastName}",
                StartedAt = assignment.StartedAt,
                TimeLimitMinutes = assignment.Test.TimeLimitMinutes,
                CurrentQuestionIndex = existingResult.CurrentQuestionIndex,
                Questions = questions,
                ExistingAnswers = existingResult.Answers
            };

            return View(model);
        }

        // ============================================
        // SAVE PROGRESS
        // ============================================
        [HttpPost]
        [AllowAnonymousTest]
        public async Task<IActionResult> SaveProgress([FromBody] SaveProgressDto dto)
        {
            var assignment = await _ctx.StudentTests
                .FirstOrDefaultAsync(st => st.LoginId == dto.LoginId);

            if (assignment == null)
                return NotFound(new { message = "Test nebyl nalezen." });

            if (assignment.FinishedAt != DateTime.MinValue)
                return BadRequest(new { message = "Test byl již dokončen." });

            assignment.ResultSnapshot = JsonSerializer.Serialize(new TestResultSnapshot
            {
                Answers = dto.Answers,
                CurrentQuestionIndex = dto.CurrentQuestionIndex
            });
            await _ctx.SaveChangesAsync();

            return Ok(new { message = "Průběh uložen." });
        }

        // ============================================
        // FINISH TEST
        // ============================================
        [HttpPost]
        [AllowAnonymousTest]
        public async Task<IActionResult> FinishTest([FromBody] SaveProgressDto dto)
        {
            var assignment = await _ctx.StudentTests
                .Include(st => st.Test)
                .FirstOrDefaultAsync(st => st.LoginId == dto.LoginId);

            if (assignment == null)
                return NotFound(new { message = "Test nebyl nalezen." });

            if (assignment.FinishedAt != DateTime.MinValue)
                return BadRequest(new { message = "Test byl již dokončen." });

            assignment.ResultSnapshot = JsonSerializer.Serialize(new TestResultSnapshot
            {
                Answers = dto.Answers
            });
            assignment.FinishedAt = DateTime.Now;
            await _ctx.SaveChangesAsync();

            var questions = JsonSerializer.Deserialize<List<QuestionSnapshotItem>>(
                assignment.Test.QuestionSnapshot) ?? [];

            int correct = 0;
            int total = questions.Count;

            foreach (var q in questions)
            {
                var answer = dto.Answers.FirstOrDefault(a => a.QuestionId == q.QuestionId);
                if (answer != null)
                {
                    var correctOptions = q.Options.Where(o => o.IsCorrect).Select(o => o.Text).OrderBy(t => t).ToList();
                    var selectedOptions = answer.SelectedOptions.OrderBy(t => t).ToList();
                    if (correctOptions.SequenceEqual(selectedOptions)) correct++;
                }
            }

            int successPct = total > 0 ? (int)Math.Round((double)correct / total * 100) : 0;

            return Ok(new { message = "Test byl úspěšně dokončen!", correct, total, successPct });
        }

        // ============================================
        // ACTIVATE / DEACTIVATE
        // ============================================
        [HttpPost]
        public async Task<IActionResult> Activate([FromBody] List<int> ids)
        {
            var tests = await _ctx.Tests.Where(t => ids.Contains(t.TestId)).ToListAsync();
            tests.ForEach(t => t.IsActive = true);
            await _ctx.SaveChangesAsync();
            return Ok(new { message = $"{tests.Count} testů aktivováno." });
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate([FromBody] List<int> ids)
        {
            var tests = await _ctx.Tests.Where(t => ids.Contains(t.TestId)).ToListAsync();
            tests.ForEach(t => t.IsActive = false);
            await _ctx.SaveChangesAsync();
            return Ok(new { message = $"{tests.Count} testů deaktivováno." });
        }

        // ============================================
        // DELETE
        // ============================================
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            var tests = await _ctx.Tests
                .Include(t => t.StudentTests)
                .Where(t => ids.Contains(t.TestId))
                .ToListAsync();

            if (tests.Count == 0)
                return NotFound(new { message = "Žádné testy k smazání nebyly nalezeny." });

            // Smazat přiřazení studentů
            var assignments = tests.SelectMany(t => t.StudentTests).ToList();
            _ctx.StudentTests.RemoveRange(assignments);

            _ctx.Tests.RemoveRange(tests);
            await _ctx.SaveChangesAsync();

            return Ok(new { message = $"{tests.Count} testů bylo smazáno." });
        }

        // ============================================
        // TOGGLE ACTIVE
        // ============================================
        [HttpPost]
        public async Task<IActionResult> ToggleActive([FromBody] int id)
        {
            var test = await _ctx.Tests.FindAsync(id);
            if (test == null)
                return NotFound(new { message = "Test nebyl nalezen." });

            test.IsActive = !test.IsActive;
            await _ctx.SaveChangesAsync();

            var status = test.IsActive ? "aktivován" : "deaktivován";
            return Ok(new { message = $"Test byl {status}.", isActive = test.IsActive });
        }

        // ============================================
        // CREATE
        // ============================================

        public async Task<IActionResult> Create()
        {
            try
            {
                CreateViewModel model = new()
                {
                    Classes = await _lookup.GetAllClassesAsync(),
                    Questions = await _ctx.Questions
                        .AsNoTracking()
                        .Include(e => e.QuestionType)
                        .Include(e => e.QuestionOptions)
                        .Where(e => e.IsActive == true)
                        .ToListAsync(),
                    Fields = await _lookup.GetActiveFieldsAsync(),
                    Students = await _ctx.Students.AsNoTracking().ToListAsync(),
                    Types = await _lookup.GetActiveTypesAsync()
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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { message = "Neplatná vstupní data.", errors });
            }

            using var transaction = await _ctx.Database.BeginTransactionAsync();

            try
            {
                var questions = await _ctx.Questions
                    .Include(q => q.QuestionOptions)
                    .Include(q => q.QuestionType)
                    .Where(q => dto.QuestionIds.Contains(q.QuestionId))
                    .ToListAsync();

                if (questions.Count != dto.QuestionIds.Count)
                    return BadRequest(new { message = "Některé vybrané otázky neexistují." });

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

                var test = new Test
                {
                    Name = dto.Name.Trim(),
                    CreatorId = (int)HttpContext.Items["CurrentUserId"],
                    StudentFieldId = dto.StudentFieldId,
                    QuestionSnapshot = snapshotJson,
                    TimeLimitMinutes = dto.TimeLimit,
                    ShuffleQuestions = true,
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

        // ============================================
        // HELPERS
        // ============================================
        private string GenerateLoginId()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            return new string(Enumerable.Range(0, 8)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }
    }
}
