using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SPSUL.ViewComponents
{
    public class ConfigViewComponent : ViewComponent
    {
        private readonly SpsulContext _ctx;
        private readonly IMemoryCache _cache;

        public ConfigViewComponent(SpsulContext ctx, IMemoryCache cache)
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

            ConfigurationViewModel model = new()
            {
                Teacher = await _ctx.Teachers.Where(e => e.TeacherId == teacherId).FirstOrDefaultAsync(),
                Title = await _ctx.TeacherTitles.Include(e => e.Title)
                .Where(e => e.TeacherId == teacherId).FirstOrDefaultAsync(),

            };

            return View(model);
        }
    }
    public class ConfigurationViewModel
    {
        public Teacher Teacher { get; set; }
        public TeacherTitle? Title { get; set; }
    }
}
