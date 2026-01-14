using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using System.Linq;
using SPSUL.Models.Data;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models.Display.Quest;
using SPSUL.Models.Display;

namespace SPSUL.Controllers
{
    public class QuestionController : Controller
    {

        private readonly SpsulContext _ctx;
        public QuestionController(SpsulContext ctx)
        {
            _ctx = ctx;
        }
        
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _ctx.QuestionRow.AsNoTracking();

                int count = await query.CountAsync();

                var rows = await query
                    .OrderByDescending(q => q.QuestionId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var model = new PaginatedList<QuestionRow>(rows, count, pageNumber, pageSize);

                return View(model);
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
                QuestionTypes = await _ctx.QuestionTypes.Where(e => e.IsActive == true).ToListAsync(),
                StudentFields = await _ctx.StudentFields.Where(e => e.IsActive == true).ToListAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var question = await _ctx.Questions
                .Include(q => q.QuestionOptions)
                .Include(q => q.QuestionType)
                .Include(q => q.Field)
                .FirstOrDefaultAsync(q => q.QuestionId == id);

            if (question == null)
            {
                return NotFound();
            }

            QuestEditVM model = new()
            {
                Question = question,
                QuestionTypes = await _ctx.QuestionTypes.Where(e => e.IsActive == true).ToListAsync(),
                StudentFields = await _ctx.StudentFields.Where(e => e.IsActive == true).ToListAsync()
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

                int? currentTeacherId = HttpContext.Session.GetInt32("TeacherId");

                if(currentTeacherId == null)
                {
                    return Unauthorized(new { message = "Uživatel není přihlášen." });
                }

                var question = new Question
                {
                    Header = dto.Header,
                    Description = dto.Description,
                    QuestionTypeId = dto.QuestionTypeId,
                    CreatorId = currentTeacherId.Value,
                    IsActive = true,
                    FieldId = dto.FieldId,
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

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Question? question = await _ctx.Questions
                .Include(q => q.QuestionOptions)
                .Include(q => q.QuestionType)
                .Include(q => q.Field)
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
                question.FieldId = dto.FieldId;
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
}
