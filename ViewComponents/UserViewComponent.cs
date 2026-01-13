using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SPSUL.ViewComponents
{
    public class UserViewComponent : ViewComponent
    {
        private readonly SpsulContext _ctx;
        private readonly IMemoryCache _cache;
        public UserViewComponent(SpsulContext ctx, IMemoryCache cache)
        {
            _ctx = ctx;
            _cache = cache;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int? teacherId = HttpContext.Session.GetInt32("TeacherId");

            if(teacherId == null)
            {
                return View("Views/Auth/Login");
            }

            if(!_cache.TryGetValue($"UserNavbar_{teacherId.Value}", out UserNavbarViewModel viewModel))
            {
                Teacher teacher = await _ctx.Teachers.FindAsync(teacherId.Value);
                List<TeacherTitle> titles = await _ctx.TeacherTitles.Where(e => e.TeacherId == teacher.TeacherId).Include(e => e.Title).ToListAsync();

                viewModel = new()
                {
                    Teacher = teacher,
                    Title = titles
                };

                _cache.Set($"UserNavbar_{teacherId.Value}", viewModel, TimeSpan.FromMinutes(10));
            }
           
            return View(viewModel);
        }
    }
    public class UserNavbarViewModel
    {
        public Teacher Teacher { get; set; }
        public List<TeacherTitle>? Title { get; set; }
    }
}