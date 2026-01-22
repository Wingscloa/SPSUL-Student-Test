using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;

namespace SPSUL.Controllers.API
{
    public class QuestionViewController : Controller
    {
        private readonly SpsulContext _ctx;
        const string partialText = "Views/Shared/QuestionType/SelectText/";
        const string partialImage = "Views/Shared/QuestionType/SelectImage/";

        public QuestionViewController(SpsulContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> Preview(int questionTypeId, int count)
        {
            if(count == 0) { return BadRequest("Počet nemůže být nula."); }
            try
            {
                QuestionType? questType = await _ctx.QuestionTypes.FindAsync(questionTypeId);

                if (questType == null) { return NotFound("Typ otázky nebyl nalezen, nemohu vygenerovat možnosti"); }

                if (questType.Name == "Uzavřená otázka")
                {
                    return PartialView(partialText + "_SelectTextPreview", count);
                }
                else if (questType.Name == "Uzavřená otázka s obrázky")
                {
                    return PartialView(partialImage + "_SelectImagePreview", count);
                }
                else
                {
                    return NotFound("Nemohl jsem najít šablonu.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Nastala chyba na serveru. Zkuste to později.");
            }
        }
        [Route("api/QuestionView/AnswerOption")]
        [HttpPost]
        public async Task<IActionResult> AnswerOption([FromBody] AnswerOptionRequest model)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest("Neplatná data.");
            }

            if (model.Count == 0) { return BadRequest("Počet nemůže být nula."); }
            try
            {
                QuestionType? questType = await _ctx.QuestionTypes.FindAsync(model.QuestionTypeId);

                if (questType == null) { return NotFound("Typ otázky nebyl nalezen, nemohu vygenerovat možnosti"); }

                if (questType.Name == "Uzavřená otázka")
                {
                    return PartialView(partialText + "_SelectTextAnswerOption", model.Count);
                }
                else if (questType.Name == "Uzavřená otázka s obrázky")
                {
                    return PartialView(partialImage + "_SelectImageAnswerOption", model.Count);
                }
                else
                {
                    return NotFound("Nemohl jsem najít šablonu.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Nastala chyba na serveru. Zkuste to později.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PreviewOptions(int questionTypeId, int count)
        {
            if(count == 0) { return BadRequest("Počet nemůže být nula."); }
            
            try
            {
                QuestionType? questType = await _ctx.QuestionTypes.FindAsync(questionTypeId);

                if(questType == null) { return NotFound("Typ otázky nebyl nalezen, nemohu vygenerovat možnosti"); }

                if(questType.Name == "Uzavřená otázka")
                {
                    return PartialView(partialText + "_SelectTextPreviewOption", count);
                }
                else if(questType.Name == "Uzavřená otázka s obrázky")
                {
                    return PartialView(partialImage + "_SelectImagePreviewOption", count);
                }
                else
                {
                    return NotFound("Nemohl jsem najít šablonu.");
                }
            }
            catch(Exception ex)
            {
                return BadRequest("Nastala chyba na serveru. Zkuste to později.");
            }
        }
    }

    public class AnswerOptionRequest
    {
        public int QuestionTypeId { get; set; }
        public int Count { get; set; }
    }
}
