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
            return statusCode switch
            {
                403 => View("Forbidden"),
                404 => View("NotFound"),
                _ => View("Error500")
            };
        }
    }
}
