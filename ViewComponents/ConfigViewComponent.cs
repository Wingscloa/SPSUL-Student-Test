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
        private readonly SharedService _sharedService;
        public ConfigViewComponent(SpsulContext ctx, IMemoryCache cache, SharedService sharedService)
        {
            _ctx = ctx;
            _cache = cache;
            _sharedService = sharedService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var teacherId = _sharedService.GetTeacherId();
            ConfigurationViewModel model = new()
            {
                Name = await _sharedService.GetNameAsync() ?? string.Empty,
                Nickname = await _ctx.Teachers.Where(e => e.TeacherId == teacherId).Select(e => e.NickName).FirstOrDefaultAsync() ?? string.Empty
            };
            return View(model);
        }
    }
    public class ConfigurationViewModel
    {
        public  required string Name { get; set; }
        public required string Nickname { get; set; } 
    }
}
