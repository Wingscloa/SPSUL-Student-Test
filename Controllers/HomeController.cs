using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models;
using SPSUL.Models.Display;
using SPSUL.Models.Display.TestModels;
using System.Text.Json;

namespace SPSUL.Controllers
{
    /// <summary>
    /// Dashboard – úvodní stránka po přihlášení.
    ///
    /// Zobrazuje:
    ///   - Statistické karty (aktivní testy, studenti, průměrná úspěšnost, čekající)
    ///   - Posledních 10 dokončených testů (aktivita)
    ///   - Chystající se testy (aktivní s čekajícími studenty)
    ///   - Sloupcový graf průměrné úspěšnosti za posledních 7 měsíců
    ///
    /// Data jsou předána přes silně typovaný HomeViewModel (ne ViewBag).
    /// </summary>
    [LoginRequired]
    public class HomeController : Controller
    {
        private readonly SpsulContext _ctx;
        public HomeController(ILogger<HomeController> logger, SharedService sharedService, SpsulContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IActionResult> Index()
        {
            var teacherId = (int?)HttpContext.Items["CurrentUserId"] ?? 0;
            var teacher = await _ctx.Teachers.AsNoTracking().FirstOrDefaultAsync(t => t.TeacherId == teacherId);

            var activeTests = await _ctx.Tests.AsNoTracking().CountAsync(t => t.IsActive);
            var totalStudents = await _ctx.Students.AsNoTracking().CountAsync(s => s.IsActive);
            var totalQuestions = await _ctx.Questions.AsNoTracking().CountAsync(q => q.IsActive);

            var finishedAssignments = await _ctx.StudentTests
                .AsNoTracking()
                .Include(st => st.Test)
                .Where(st => st.FinishedAt != DateTime.MinValue)
                .ToListAsync();

            double avgSuccess = 0;
            if (finishedAssignments.Count > 0)
            {
                avgSuccess = finishedAssignments.Average(st => CalculateSuccess(st));
            }

            var pendingCount = await _ctx.StudentTests
                .AsNoTracking()
                .CountAsync(st => st.StartedAt == DateTime.MinValue);

            var recentFinished = await _ctx.StudentTests
                .AsNoTracking()
                .Include(st => st.Test)
                .Include(st => st.Student)
                .Where(st => st.FinishedAt != DateTime.MinValue)
                .OrderByDescending(st => st.FinishedAt)
                .Take(10)
                .ToListAsync();

            var upcomingTests = await _ctx.Tests
                .AsNoTracking()
                .Include(t => t.StudentField)
                .Include(t => t.StudentTests)
                .Where(t => t.IsActive && t.StudentTests.Any(st => st.StartedAt == DateTime.MinValue))
                .OrderByDescending(t => t.TestId)
                .Take(5)
                .ToListAsync();

            var sevenMonthsAgo = DateTime.Now.AddMonths(-6);
            var monthlyData = finishedAssignments
                .Where(st => st.FinishedAt >= sevenMonthsAgo)
                .GroupBy(st => new { st.FinishedAt.Year, st.FinishedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Avg = g.Average(st => (double)CalculateSuccess(st))
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            var vm = new HomeViewModel
            {
                TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : "Učiteli",
                ActiveTests = activeTests,
                TotalStudents = totalStudents,
                TotalQuestions = totalQuestions,
                AvgSuccess = avgSuccess,
                PendingCount = pendingCount,
                RecentFinished = recentFinished,
                UpcomingTests = upcomingTests,
                MonthlyLabels = JsonSerializer.Serialize(
                    monthlyData.Select(m => $"{m.Month}/{m.Year}").ToList()),
                MonthlyValues = JsonSerializer.Serialize(
                    monthlyData.Select(m => Math.Round(m.Avg, 1)).ToList())
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private int CalculateSuccess(Models.Data.StudentTest st)
        {
            try
            {
                var questions = JsonSerializer.Deserialize<List<QuestionSnapshotItem>>(
                    st.Test.QuestionSnapshot) ?? [];
                var result = JsonSerializer.Deserialize<TestResultSnapshot>(
                    st.ResultSnapshot) ?? new();

                int correct = 0;
                foreach (var q in questions)
                {
                    var answer = result.Answers.FirstOrDefault(a => a.QuestionId == q.QuestionId);
                    if (answer != null)
                    {
                        var cOpts = q.Options.Where(o => o.IsCorrect).Select(o => o.Text).OrderBy(t => t).ToList();
                        var sOpts = answer.SelectedOptions.OrderBy(t => t).ToList();
                        if (cOpts.SequenceEqual(sOpts)) correct++;
                    }
                }
                return questions.Count > 0 ? (int)Math.Round((double)correct / questions.Count * 100) : 0;
            }
            catch { return 0; }
        }
    }
}
