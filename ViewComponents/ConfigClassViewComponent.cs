using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models.Display;

namespace SPSUL.ViewComponents
{
    public class ConfigClassViewComponent : ViewComponent
    {
        private readonly SpsulContext _ctx;
        public ConfigClassViewComponent(SpsulContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IViewComponentResult> InvokeAsync(int pageNumber = 1, int pageSize = 13)
        {
            List<Classes> query = await _ctx.Classes.Include(c => c.ClassesFields).ThenInclude(e => e.StudentField).ToListAsync();

            List<Classes> rows = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(c => c.ClassesId)
                .ToList();

            int totalCount = query.Count;

            ConfigClassVM model = new()
            {
                Classes = new PaginatedList<Classes>(rows, totalCount, pageNumber, pageSize),
                Fields = await _ctx.StudentFields.ToListAsync()
            };

            return View(model);
        }
    }
    public class ConfigClassVM
    {
        public PaginatedList<Classes> Classes { get; set; }
        public List<StudentField> Fields { get; set; }
    }
}
