using Microsoft.AspNetCore.Mvc;
using SPSUL.Models.Data;
using SPSUL.Models;
namespace SPSUL.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SpsulContext _ctx;
        
        public StudentsController(SpsulContext ctx)
        {
            _ctx = ctx;
        }
        public IActionResult Index(bool? active)
        {
            List<Student> model = _ctx.Students.Where(e => e.IsActive == false).ToList();
            return View(model);
        }
    }
}
