using Microsoft.AspNetCore.Mvc;

namespace SPSUL.Controllers.API
{
    public class ConfigController : Controller
    {
        [HttpGet("/api/config/section/{sectionName}")]
        public IActionResult LoadSection(string sectionName)
        {
            return ViewComponent(sectionName);
        }
    }
}
