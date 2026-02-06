using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models;
using SPSUL.Models.Display.TestModels;
using System.Text.Json;

namespace SPSUL.Controllers
{
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
            var teacher = await _ctx.Teachers.FindAsync(teacherId);

            // Stat cards
            var activeTests = await _ctx.Tests.CountAsync(t => t.IsActive);
            var totalStudents = await _ctx.Students.CountAsync(s => s.IsActive);
            var totalQuestions = await _ctx.Questions.CountAsync(q => q.IsActive);

            // Finished tests for average
            var finishedAssignments = await _ctx.StudentTests
                .Include(st => st.Test)
                .Where(st => st.FinishedAt != DateTime.MinValue)
                .ToListAsync();

            double avgSuccess = 0;
            if (finishedAssignments.Count > 0)
            {
                avgSuccess = finishedAssignments.Average(st => CalculateSuccess(st));
            }

            // Pending (assigned but not started)
            var pendingCount = await _ctx.StudentTests
                .CountAsync(st => st.StartedAt == DateTime.MinValue);

            // Recent activity - last 10 finished tests
            var recentFinished = await _ctx.StudentTests
                .Include(st => st.Test)
                .Include(st => st.Student)
                .Where(st => st.FinishedAt != DateTime.MinValue)
                .OrderByDescending(st => st.FinishedAt)
                .Take(10)
                .ToListAsync();

            // Upcoming - active tests with pending students
            var upcomingTests = await _ctx.Tests
                .Include(t => t.StudentField)
                .Include(t => t.StudentTests)
                .Where(t => t.IsActive && t.StudentTests.Any(st => st.StartedAt == DateTime.MinValue))
                .OrderByDescending(t => t.TestId)
                .Take(5)
                .ToListAsync();

            // Chart data - last 7 months avg success
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

            ViewBag.TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : "Uèiteli";
            ViewBag.ActiveTests = activeTests;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalQuestions = totalQuestions;
            ViewBag.AvgSuccess = avgSuccess;
            ViewBag.PendingCount = pendingCount;
            ViewBag.RecentFinished = recentFinished;
            ViewBag.UpcomingTests = upcomingTests;
            ViewBag.MonthlyLabels = JsonSerializer.Serialize(
                monthlyData.Select(m => $"{m.Month}/{m.Year}").ToList());
            ViewBag.MonthlyValues = JsonSerializer.Serialize(
                monthlyData.Select(m => Math.Round(m.Avg, 1)).ToList());

            return View();
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
