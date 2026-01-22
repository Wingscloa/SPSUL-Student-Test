using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Display;
using SPSUL.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SPSUL.ViewComponents
{
    public class ConfigTeacherViewComponent : ViewComponent
    {
        private readonly SpsulContext _ctx;
        private readonly IMemoryCache _cache;
        public ConfigTeacherViewComponent(SpsulContext ctx, IMemoryCache cache)
        {
            _ctx = ctx;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(int pageNumber = 1, int pageSize = 13)
        {
            List<Teacher> query = await _ctx.Teachers.Include(e => e.Titles).ThenInclude(e => e.Title).ToListAsync();

            List<Teacher> rows = query.Skip((pageNumber - 1) * pageSize)
                .OrderByDescending(t => t.TeacherId)
                .Take(pageSize)
                .ToList();

            int count = query.Count;

            ConfigTeacherModel model = new ConfigTeacherModel
            {
                Teachers = new PaginatedList<Teacher>(rows, count, pageNumber, pageSize),
                Roles = await _ctx.Roles.ToListAsync(),
                Titles = await _ctx.Titles.ToListAsync()
            };

            return View(model);
        }
    }

    public class ConfigTeacherModel
    {
        public required PaginatedList<Teacher> Teachers { get; set; }
        public required List<Role> Roles { get; set; }
        public required List<Title> Titles { get; set; }
    }
}
