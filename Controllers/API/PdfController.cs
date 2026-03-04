using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display.StudentModels;
using SPSUL.Models.Display.TeacherModels;
using SPSUL.Models.Display.ClassesModels;

using SPSUL.Models.Display.QuestionModels;

namespace SPSUL.Controllers.API
{
    /// <summary>
    /// API controller pro generování PDF dokumentù pomocí knihovny QuestPDF.
    ///
    /// Endpointy:
    ///   GET /api/pdf/tests    – export seznamu testù do PDF
    ///   GET /api/pdf/students – export seznamu studentù do PDF
    ///   GET /api/pdf/teachers – export seznamu uèitelù do PDF
    ///   GET /api/pdf/classes  – export seznamu tøíd do PDF
    ///
    /// Jak funguje:
    ///   PdfService (QuestPDF) pøijme data a vygeneruje PDF binar.ní soubor.
    ///   Response je vrácena jako application/pdf s Content-Disposition: attachment.
    ///   Prohlížeè automaticky spustí stažení souboru.
    /// </summary>
    [LoginRequired]
    public class PdfController : Controller
    {
        private readonly SpsulContext _ctx;
        private readonly PdfService _pdf;

        public PdfController(SpsulContext ctx, PdfService pdf)
        {
            _ctx = ctx;
            _pdf = pdf;
        }

        // ============================================
        // STUDENTS PDF (filtered)
        // ============================================
        [HttpPost]
        [Route("api/pdf/students")]
        public async Task<IActionResult> Students([FromBody] StudentFilter model)
        {
            var query = await _ctx.Students.Where(e =>
                (model.SearchFilter != null
                    ? (e.FirstName.Contains(model.SearchFilter) || e.LastName.Contains(model.SearchFilter))
                    : true) &&
                (model.ActiveFilter.HasValue ? e.IsActive == model.ActiveFilter.Value : true))
                .Include(e => e.ClassesStudents).ThenInclude(e => e.Classes)
                .Include(e => e.StudentTests).ThenInclude(e => e.Test)
                .ToListAsync();

            if (model.ClassFilterIds != null && model.ClassFilterIds.Count > 0)
                query = query.Where(e => e.ClassesStudents.Any(t => model.ClassFilterIds.Contains(t.ClassesId))).ToList();

            if (model.TestFilterIds != null && model.TestFilterIds.Count > 0)
                query = query.Where(e => e.StudentTests.Any(t => model.TestFilterIds.Contains(t.TestId))).ToList();

            var students = query.OrderByDescending(e => e.StudentId).ToList();

            var pdfBytes = _pdf.GenerateStudentsPdf(students);
            return File(pdfBytes, "application/pdf", "Studenti.pdf");
        }

        // ============================================
        // TEACHERS PDF (filtered)
        // ============================================
        [HttpPost]
        [Route("api/pdf/teachers")]
        public async Task<IActionResult> Teachers([FromBody] TeacherFilter model)
        {
            var query = await _ctx.Teachers.Where(e =>
                (model.SearchFilter != null
                    ? (e.FirstName.Contains(model.SearchFilter) || e.LastName.Contains(model.SearchFilter) || e.NickName.Contains(model.SearchFilter))
                    : true) &&
                (model.ActiveFilter.HasValue ? e.IsActive == model.ActiveFilter.Value : true))
                .Include(e => e.Titles).ThenInclude(e => e.Title)
                .Include(e => e.TeacherRoles)
                .ToListAsync();

            if (model.TitleFilterIds != null && model.TitleFilterIds.Count > 0)
                query = query.Where(e => e.Titles.Any(t => model.TitleFilterIds.Contains(t.TitleId))).ToList();

            if (model.RoleFilterIds != null && model.RoleFilterIds.Count > 0)
                query = query.Where(e => e.TeacherRoles.Any(r => model.RoleFilterIds.Contains(r.RoleId))).ToList();

            var teachers = query.OrderByDescending(e => e.TeacherId).ToList();

            var pdfBytes = _pdf.GenerateTeachersPdf(teachers);
            return File(pdfBytes, "application/pdf", "Ucitele.pdf");
        }

        // ============================================
        // CLASSES PDF (filtered)
        // ============================================
        [HttpPost]
        [Route("api/pdf/classes")]
        public async Task<IActionResult> Classes([FromBody] ClassesFilter model)
        {
            var query = await _ctx.Classes.Where(e =>
                (model.SearchFilter != null ? e.Name.Contains(model.SearchFilter) : true) &&
                (model.ActiveFilter.HasValue ? e.IsActive == model.ActiveFilter.Value : true) &&
                (model.StartFromFilter.HasValue ? e.StartFrom == model.StartFromFilter.Value : true) &&
                (model.EndToFilter.HasValue ? e.EndTo == model.EndToFilter.Value : true))
                .Include(e => e.ClassesFields).ThenInclude(e => e.StudentField)
                .ToListAsync();

            if (model.FieldFilterIds != null && model.FieldFilterIds.Count > 0)
                query = query.Where(e => e.ClassesFields.Any(t => model.FieldFilterIds.Contains(t.StudentFieldId))).ToList();

            var classes = query.OrderByDescending(e => e.ClassesId).ToList();

            var pdfBytes = _pdf.GenerateClassesPdf(classes);
            return File(pdfBytes, "application/pdf", "Tridy.pdf");
        }

        // ============================================
        // TESTS PDF
        // ============================================
        [HttpGet]
        [Route("api/pdf/tests")]
        public async Task<IActionResult> Tests()
        {
            var tests = await _ctx.Tests
                .Include(t => t.Creator)
                .Include(t => t.StudentField)
                .OrderByDescending(t => t.TestId)
                .ToListAsync();

            var pdfBytes = _pdf.GenerateTestsPdf(tests);
            return File(pdfBytes, "application/pdf", "Testy.pdf");
        }

        // ============================================
        // QUESTIONS PDF (filtered)
        // ============================================
        [HttpPost]
        [Route("api/pdf/questions")]
        public async Task<IActionResult> Questions([FromBody] QuestionFilter model)
        {
            var query = _ctx.Questions
                .Include(q => q.Creator)
                .Include(q => q.QuestionType)
                .Include(q => q.Field)
                .AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchFilter))
                query = query.Where(q => q.Header.Contains(model.SearchFilter));

            if (model.ActiveFilter.HasValue)
                query = query.Where(q => q.IsActive == model.ActiveFilter.Value);

            if (model.CreatorId.HasValue)
                query = query.Where(q => q.CreatorId == model.CreatorId.Value);

            if (model.QuestionTypeId.HasValue)
                query = query.Where(q => q.QuestionTypeId == model.QuestionTypeId.Value);

            if (model.FieldId.HasValue)
                query = query.Where(q => q.FieldId == model.FieldId.Value);

            var questions = await query.OrderByDescending(q => q.QuestionId).ToListAsync();

            var pdfBytes = _pdf.GenerateQuestionsPdf(questions);
            return File(pdfBytes, "application/pdf", "Otazky.pdf");
        }
    }
}
