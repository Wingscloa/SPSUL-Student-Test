using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using System.Linq;
using SPSUL.Models.Data;
using SPSUL.Models.Display.Question;
using Microsoft.EntityFrameworkCore;

namespace SPSUL.Controllers
{
    public class QuestionController : Controller
    {

        private readonly SpsulContext _ctx;
        public QuestionController(SpsulContext ctx)
        {
            _ctx = ctx;
        }
        
        public async Task<IActionResult> Index()
        {
            try
            {
                var questions = _ctx.Questions.Select(q => new QuestionRow
                {
                    Id = q.QuestionId,
                    Nazev = q.Header,
                    Tvurce = string.Join(' ', q.Creator.Titles) + q.Creator.FirstName + q.Creator.LastName,
                    Aktivni = q.IsActive,
                    PocetPrirazeni = -1,
                    PrumerUspech = 0
                }).ToList();
                return View(questions);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }
        public async Task<IActionResult> Create()
        {
            QuestCreateVM model = new()
            {
                QuestionTypes = await _ctx.QuestionTypes.Where(e => e.IsActive == true).ToListAsync()
            };
            return View(model);
        }
        // API (CURD)
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Question? question = await _ctx.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Question question)
        {
            try
            {
                Question before = await _ctx.Questions.FindAsync(question.QuestionId);
                if (before == null)
                {
                    _ctx.Questions.Add(question);
                }
                else
                {
                    before.Header = question.Header;
                    before.Description = question.Description;
                    before.IsActive = question.IsActive;
                    before.QuestionTypeId = question.QuestionTypeId;
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Question? question = await _ctx.Questions.FindAsync(id);
                if (question != null)
                {
                    _ctx.Questions.Remove(question);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
    public class QuestionRow
    {
        public int Id { get; set; }
        public string Nazev { get; set; } = string.Empty;
        public string Tvurce { get; set; } = string.Empty;
        public bool Aktivni { get; set; }
        public int PocetPrirazeni { get; set; }
        public int PrumerUspech { get; set; }
    }
}
