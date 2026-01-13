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
        
        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // TODO: Get current teacher ID from session/auth
                int currentTeacherId = 1; // Placeholder

                var question = new Question
                {
                    Header = dto.Header,
                    Description = dto.Description,
                    QuestionTypeId = dto.QuestionTypeId,
                    CreatorId = currentTeacherId,
                    IsActive = true,
                    QuestionOptions = dto.Options.Select(o => new QuestionOption
                    {
                        Text = o.Text,
                        ImageBase64 = o.ImageBase64 ?? string.Empty,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                };

                _ctx.Questions.Add(question);
                await _ctx.SaveChangesAsync();

                return Ok(new { questionId = question.QuestionId, message = "Otázka byla úspěšně vytvořena!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Chyba při vytváření otázky: " + ex.Message });
            }
        }

        // API (CURD)
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Question? question = await _ctx.Questions
                .Include(q => q.QuestionOptions)
                .Include(q => q.QuestionType)
                .FirstOrDefaultAsync(q => q.QuestionId == id);
            
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] QuestionUpdateDto dto)
        {
            try
            {
                var question = await _ctx.Questions
                    .Include(q => q.QuestionOptions)
                    .FirstOrDefaultAsync(q => q.QuestionId == dto.QuestionId);

                if (question == null)
                {
                    return NotFound(new { message = "Otázka nebyla nalezena." });
                }

                question.Header = dto.Header;
                question.Description = dto.Description;
                question.QuestionTypeId = dto.QuestionTypeId;
                question.IsActive = dto.IsActive;

                // Remove old options
                _ctx.QuestionOptions.RemoveRange(question.QuestionOptions);

                // Add new options
                question.QuestionOptions = dto.Options.Select(o => new QuestionOption
                {
                    QuestionId = question.QuestionId,
                    Text = o.Text,
                    ImageBase64 = o.ImageBase64 ?? string.Empty,
                    IsCorrect = o.IsCorrect
                }).ToList();

                await _ctx.SaveChangesAsync();

                return Ok(new { message = "Otázka byla úspěšně aktualizována!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Chyba při aktualizaci otázky: " + ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Question? question = await _ctx.Questions
                    .Include(q => q.QuestionOptions)
                    .FirstOrDefaultAsync(q => q.QuestionId == id);
                
                if (question != null)
                {
                    _ctx.Questions.Remove(question);
                    await _ctx.SaveChangesAsync();
                    return Ok(new { message = "Otázka byla úspěšně smazána!" });
                }
                return NotFound(new { message = "Otázka nebyla nalezena." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Chyba při mazání otázky: " + ex.Message });
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

    public class QuestionCreateDto
    {
        public string Header { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int QuestionTypeId { get; set; }
        public List<QuestionOptionDto> Options { get; set; } = new();
    }

    public class QuestionUpdateDto : QuestionCreateDto
    {
        public int QuestionId { get; set; }
        public bool IsActive { get; set; }
    }

    public class QuestionOptionDto
    {
        public string Text { get; set; } = string.Empty;
        public string? ImageBase64 { get; set; }
        public bool IsCorrect { get; set; }
    }
}
