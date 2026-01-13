using Microsoft.AspNetCore.Mvc;

namespace SPSUL.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View("Error500");
        }

        [Route("{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            if(statusCode == 404)
            {
                return View("NotFound");
            }
            return View("Error500");
        }
    }
}
