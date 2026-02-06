using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display.TestModels;
using SPSUL.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SPSUL.Controllers
{
    [LoginRequired]
    public class DetailController : Controller
    {
        private readonly SpsulContext _ctx;

        public DetailController(SpsulContext ctx)
        {
            _ctx = ctx;
        }

        // GET: Detail/Index
        public async Task<IActionResult> Index(
            string? Q1,
            bool? Active1,
            string? successRange,
            bool? completed,
            DateTime? dateFrom,
            DateTime? dateTo,
            string? sortBy,
            int Page1 = 1,
            int PageSize1 = 10)
        {
            var allItems = await _ctx.StudentTests
                .Include(st => st.Test)
                .Include(st => st.Student)
                .Where(st => st.FinishedAt != DateTime.MinValue)
                .ToListAsync();

            var testData = allItems.Select(st =>
            {
                int successPct = 0;
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
                            var correctOpts = q.Options.Where(o => o.IsCorrect).Select(o => o.Text).OrderBy(t => t).ToList();
                            var selected = answer.SelectedOptions.OrderBy(t => t).ToList();
                            if (correctOpts.SequenceEqual(selected)) correct++;
                        }
                    }
                    successPct = questions.Count > 0 ? (int)Math.Round((double)correct / questions.Count * 100) : 0;
                }
                catch { }

                return new AssignedTestVm
                {
                    Id = st.TestId,
                    StudentId = st.StudentId,
                    Nazev = st.Test.Name,
                    LoginId = st.LoginId,
                    Jmeno = $"{st.Student.FirstName} {st.Student.LastName}",
                    ZacalV = st.StartedAt,
                    DokoncilV = st.FinishedAt,
                    UspechPct = successPct,
                    Absolvoval = successPct >= 50,
                    Aktivni = st.Test.IsActive
                };
            }).ToList();

            var filtered = testData.AsQueryable();

            if (!string.IsNullOrEmpty(Q1))
            {
                filtered = filtered.Where(x =>
                    x.Nazev.Contains(Q1, StringComparison.OrdinalIgnoreCase) ||
                    x.Jmeno.Contains(Q1, StringComparison.OrdinalIgnoreCase));
            }

            if (completed.HasValue)
                filtered = filtered.Where(x => x.Absolvoval == completed.Value);

            if (dateFrom.HasValue)
                filtered = filtered.Where(x => x.ZacalV >= dateFrom.Value);

            if (dateTo.HasValue)
                filtered = filtered.Where(x => x.DokoncilV <= dateTo.Value);

            filtered = sortBy switch
            {
                "date-asc" => filtered.OrderBy(x => x.DokoncilV),
                "success-desc" => filtered.OrderByDescending(x => x.UspechPct),
                "success-asc" => filtered.OrderBy(x => x.UspechPct),
                "name-asc" => filtered.OrderBy(x => x.Nazev),
                "name-desc" => filtered.OrderByDescending(x => x.Nazev),
                _ => filtered.OrderByDescending(x => x.DokoncilV)
            };

            var total = filtered.Count();
            var items = filtered
                .Skip((Page1 - 1) * PageSize1)
                .Take(PageSize1)
                .ToList();

            var model = new DetailIndexViewModel
            {
                Q1 = Q1,
                Active1 = Active1,
                Page1 = Page1,
                PageSize1 = PageSize1,
                Assigned = new PagedResult<AssignedTestVm>
                {
                    Items = items,
                    Page = Page1,
                    PageSize = PageSize1,
                    Total = total
                }
            };

            return View(model);
        }

        // GET: Detail/History?studentId=1&testId=2
        public async Task<IActionResult> History(int studentId, int testId)
        {
            var assignment = await _ctx.StudentTests
                .Include(st => st.Test)
                .Include(st => st.Student)
                .FirstOrDefaultAsync(st => st.StudentId == studentId && st.TestId == testId);

            if (assignment == null)
                return RedirectToAction("Index");

            ViewBag.Assignment = assignment;

            List<QuestionSnapshotItem> questions = [];
            TestResultSnapshot result = new();
            try
            {
                questions = JsonSerializer.Deserialize<List<QuestionSnapshotItem>>(
                    assignment.Test.QuestionSnapshot) ?? [];
                result = JsonSerializer.Deserialize<TestResultSnapshot>(
                    assignment.ResultSnapshot) ?? new();
            }
            catch { }

            ViewBag.Questions = questions;
            ViewBag.Answers = result.Answers;

            return View();
        }

        // GET: Detail/View?studentId=1&testId=2
        public async Task<IActionResult> View(int studentId, int testId)
        {
            var assignment = await _ctx.StudentTests
                .Include(st => st.Test)
                .Include(st => st.Student)
                .FirstOrDefaultAsync(st => st.StudentId == studentId && st.TestId == testId);

            if (assignment == null)
                return RedirectToAction("Index");

            ViewBag.Assignment = assignment;

            List<QuestionSnapshotItem> questions = [];
            TestResultSnapshot result = new();
            try
            {
                questions = JsonSerializer.Deserialize<List<QuestionSnapshotItem>>(
                    assignment.Test.QuestionSnapshot) ?? [];
                result = JsonSerializer.Deserialize<TestResultSnapshot>(
                    assignment.ResultSnapshot) ?? new();
            }
            catch { }

            ViewBag.Questions = questions;
            ViewBag.Answers = result.Answers;

            return View();
        }

        // ============================================
        // EXPORT CSV
        // ============================================
        public async Task<IActionResult> Export()
        {
            var allItems = await _ctx.StudentTests
                .Include(st => st.Test)
                .Include(st => st.Student)
                .Where(st => st.FinishedAt != DateTime.MinValue)
                .ToListAsync();

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Test;Student;LoginId;Zacal;Dokoncil;Cas (min);Uspesnost (%)");

            foreach (var st in allItems)
            {
                int pct = CalculateSuccess(st);
                var duration = st.FinishedAt - st.StartedAt;

                csv.AppendLine(string.Join(";",
                    st.Test.Name,
                    $"{st.Student.FirstName} {st.Student.LastName}",
                    st.LoginId,
                    st.StartedAt.ToString("dd.MM.yyyy HH:mm"),
                    st.FinishedAt.ToString("dd.MM.yyyy HH:mm"),
                    ((int)duration.TotalMinutes).ToString(),
                    pct.ToString()
                ));
            }

            var bytes = System.Text.Encoding.UTF8.GetPreamble()
                .Concat(System.Text.Encoding.UTF8.GetBytes(csv.ToString()))
                .ToArray();

            return File(bytes, "text/csv", $"vysledky_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        }

        // ============================================
        // STATS - statistiky dashboard
        // ============================================
        public async Task<IActionResult> Stats()
        {
            var allItems = await _ctx.StudentTests
                .Include(st => st.Test)
                    .ThenInclude(t => t.StudentField)
                .Include(st => st.Student)
                    .ThenInclude(s => s.ClassesStudents)
                        .ThenInclude(cs => cs.Classes)
                .Where(st => st.FinishedAt != DateTime.MinValue)
                .ToListAsync();

            // Per-test stats
            var testStats = allItems
                .GroupBy(st => new { st.TestId, st.Test.Name })
                .Select(g => new
                {
                    g.Key.Name,
                    Count = g.Count(),
                    AvgPct = g.Average(st => (double)CalculateSuccess(st)),
                    BestPct = g.Max(st => CalculateSuccess(st)),
                    WorstPct = g.Min(st => CalculateSuccess(st))
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            // Per-class stats
            var classStats = allItems
                .SelectMany(st => st.Student.ClassesStudents.Select(cs => new
                {
                    ClassName = cs.Classes?.Name ?? "Bez tøídy",
                    Pct = CalculateSuccess(st)
                }))
                .GroupBy(x => x.ClassName)
                .Select(g => new
                {
                    ClassName = g.Key,
                    Count = g.Count(),
                    AvgPct = g.Average(x => (double)x.Pct)
                })
                .OrderByDescending(x => x.AvgPct)
                .ToList();

            // Time trend (last 30 days)
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var timeTrend = allItems
                .Where(st => st.FinishedAt >= thirtyDaysAgo)
                .GroupBy(st => st.FinishedAt.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("dd.MM"),
                    Count = g.Count(),
                    AvgPct = g.Average(st => (double)CalculateSuccess(st))
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Overall
            int totalTests = allItems.Count;
            double overallAvg = totalTests > 0 ? allItems.Average(st => (double)CalculateSuccess(st)) : 0;
            int passed = allItems.Count(st => CalculateSuccess(st) >= 50);

            ViewBag.TestStats = testStats;
            ViewBag.ClassStats = classStats;
            ViewBag.TimeTrend = timeTrend;
            ViewBag.TotalTests = totalTests;
            ViewBag.OverallAvg = overallAvg;
            ViewBag.Passed = passed;
            ViewBag.Failed = totalTests - passed;

            ViewBag.TestStatsJson = JsonSerializer.Serialize(testStats);
            ViewBag.ClassStatsJson = JsonSerializer.Serialize(classStats);
            ViewBag.TimeTrendJson = JsonSerializer.Serialize(timeTrend);

            return View();
        }

        // ============================================
        // HELPER
        // ============================================
        private int CalculateSuccess(StudentTest st)
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
