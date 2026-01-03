using Microsoft.AspNetCore.Mvc;

namespace SPSUL.Controllers
{
    public class TestController : Controller
    {
        [HttpGet]
        public Task<IActionResult> LogToTest(string TestId)
        {
            return Task.FromResult<IActionResult>(RedirectToAction("Index", "TakeTest"));
        }
        public IActionResult Example()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Sample()
        {
            return View();
        }
    }
}
