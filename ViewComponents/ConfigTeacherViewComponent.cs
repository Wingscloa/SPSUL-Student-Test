using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
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

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int? teacherId = HttpContext.Session.GetInt32("TeacherId");
            if (teacherId == null)
            {
                return View("Views/Auth/Login");
            }

            ConfigTeacherViewModel model = new()
            {
                Teachers = await _ctx.Teachers
                    .Include(e => e.Titles)
                    .AsNoTracking()
                    .OrderBy(t => t.LastName)
                    .ThenBy(t => t.FirstName)
                    .ToListAsync()
            };

            return View(model);
        }
    }
    public class ConfigTeacherViewModel
    {
        public List<Teacher>? Teachers { get; set; }
    }
}
