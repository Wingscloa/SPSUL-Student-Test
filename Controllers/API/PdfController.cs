using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display.StudentModels;
using SPSUL.Models.Display.TeacherModels;
using SPSUL.Models.Display.ClassesModels;

namespace SPSUL.Controllers.API
{
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
    }
}
