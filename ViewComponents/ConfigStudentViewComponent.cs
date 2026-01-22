using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Display;
using SPSUL.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SPSUL.ViewComponents
{
    public class ConfigStudentViewComponent : ViewComponent
    {
        private readonly SpsulContext _ctx;
        private readonly IMemoryCache _cache;
        public ConfigStudentViewComponent(SpsulContext ctx, IMemoryCache cache)
        {
            _ctx = ctx;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(int pageNumber = 1, int pageSize = 13)
        {
            List<Student> query = await _ctx.Students.Include(e => e.ClassesStudents).ThenInclude(e => e.Classes).ToListAsync();

            List<Student> rows = query.Skip((pageNumber - 1) * pageSize)
                .OrderByDescending(t => t.StudentId)
                .Take(pageSize)
                .ToList();

            int count = query.Count;

            ConfigStudentViewModel model = new()
            {
                Students = new PaginatedList<Student>(rows, count, pageNumber, pageSize),
                Classes = await _ctx.Classes.ToListAsync(),
            };

            return View(model);
        }
    }
    public class ConfigStudentViewModel
    {
        public required PaginatedList<Student> Students { get; set; }
        public required List<Classes> Classes { get; set; }
    }
}
