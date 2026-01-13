using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SPSUL.ViewComponents
{
    public class ConfigProfileViewComponent : ViewComponent
    {
        private readonly SpsulContext _ctx;
        private readonly IMemoryCache _cache;
        public ConfigProfileViewComponent(SpsulContext ctx, IMemoryCache cache)
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

            ConfigProfileViewModel model = new()
            {
                Teacher = await _ctx.Teachers.Where(e => e.TeacherId == teacherId).FirstOrDefaultAsync(),
                Title = await _ctx.TeacherTitles.Include(e => e.Title)
                .Where(e => e.TeacherId == teacherId).ToListAsync(),
                Roles = await _ctx.Roles.ToListAsync(),
                TeacherRoles = await _ctx.TeacherRoles.Include(e => e.Role).Where(e => e.TeacherId == teacherId).ToListAsync(),
            };
            return View(model);
        }
    }
    public class ConfigProfileViewModel
    {
        public Teacher Teacher { get; set; }
        public List<TeacherTitle>? Title { get; set; }
        public List<Role> Roles { get; set; }
        public List<TeacherRole>? TeacherRoles { get; set; } 
    }
}
