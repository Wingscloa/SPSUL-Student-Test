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
        private readonly PdfService _pdf;

        public DetailController(SpsulContext ctx, PdfService pdf)
        {
            _ctx = ctx;
            _pdf = pdf;
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
                .AsNoTracking()
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
        // EXPORT PDF
        // ============================================
        public async Task<IActionResult> ExportPdf(
            string? Q1,
            bool? completed,
            DateTime? dateFrom,
            DateTime? dateTo,
            string? sortBy)
        {
            var allItems = await _ctx.StudentTests
                .Include(st => st.Test)
                .Include(st => st.Student)
                .Where(st => st.FinishedAt != DateTime.MinValue)
                .ToListAsync();

            var testData = allItems.Select(st =>
            {
                int successPct = CalculateSuccess(st);
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
            }).AsQueryable();

            if (!string.IsNullOrEmpty(Q1))
                testData = testData.Where(x =>
                    x.Nazev.Contains(Q1, StringComparison.OrdinalIgnoreCase) ||
                    x.Jmeno.Contains(Q1, StringComparison.OrdinalIgnoreCase));

            if (completed.HasValue)
                testData = testData.Where(x => x.Absolvoval == completed.Value);

            if (dateFrom.HasValue)
                testData = testData.Where(x => x.ZacalV >= dateFrom.Value);

            if (dateTo.HasValue)
                testData = testData.Where(x => x.DokoncilV <= dateTo.Value);

            testData = sortBy switch
            {
                "date-asc" => testData.OrderBy(x => x.DokoncilV),
                "success-desc" => testData.OrderByDescending(x => x.UspechPct),
                "success-asc" => testData.OrderBy(x => x.UspechPct),
                "name-asc" => testData.OrderBy(x => x.Nazev),
                "name-desc" => testData.OrderByDescending(x => x.Nazev),
                _ => testData.OrderByDescending(x => x.DokoncilV)
            };

            var pdfBytes = _pdf.GenerateResultsPdf(testData.ToList());
            return File(pdfBytes, "application/pdf", $"Vysledky_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
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
