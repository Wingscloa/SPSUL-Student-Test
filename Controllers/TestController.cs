using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Display.Test;
using Microsoft.EntityFrameworkCore;

namespace SPSUL.Controllers
{
    public class TestController : Controller
    {

        private readonly SpsulContext _ctx;
        public TestController(SpsulContext context)
        {
            _ctx = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Test(string TestId)
        {
            if (!ModelState.IsValid)
            {
                this.Alert("Neplatný vstup identifičního čísla testu.", NotificationType.Error);
                return View("Index");
            }
            else
            {
                try
                {
                    bool testExists = await _ctx.StudentTests.AnyAsync(e => e.LoginId == TestId);

                    if(!testExists)
                    {
                        this.Alert("Zadané identifiční číslo testu neexistuje.", NotificationType.Error);
                        return Ok();
                    }

                    return View("Test",TestId);
                }
                catch (Exception ex)
                {
                    return View("Error");
                }
            }
        }
        public IActionResult Example()
        {
            return View();
        }
        public async Task<IActionResult> Create()
        {
            try
            {
                CreateViewModel model = new()
                {
                   Classes = await _ctx.Classes.Include(e => e.ClassesStudents).ToListAsync(),
                   Questions = await _ctx.Questions
                   .Include(e => e.QuestionType).Include(e => e.QuestionOptions)
                   .ToListAsync(),
                   StudentFields = await _ctx.StudentFields.ToListAsync(),
                   Students = await _ctx.Students.ToListAsync(),
                };
                return View(model);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }
    }
}
