using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SPSUL.ViewComponents
{
    public class ConfigAboutViewComponent : ViewComponent
    {
        private readonly SpsulContext _ctx;
        private readonly IMemoryCache _cache;

        public ConfigAboutViewComponent(SpsulContext ctx, IMemoryCache cache)
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

            ConfigAboutViewModel model = new();

            return View(model);
        }
    }
    public class ConfigAboutViewModel
    {

    }
}
