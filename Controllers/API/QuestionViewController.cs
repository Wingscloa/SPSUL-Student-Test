using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display.QuestionForm;
using System.Net;

namespace SPSUL.Controllers.API
{
    public class QuestionViewController : Controller
    {
        private readonly SpsulContext _ctx;

        const string partialText = "Views/Shared/QuestionType/SelectText/Create/";
        const string partialImage = "Views/Shared/QuestionType/SelectImage/Create/";
        public QuestionViewController(SpsulContext ctx)
        {
            _ctx = ctx;
        }

        public List<OptionBase> GenerateOptions(int startIndex, int count)
        {
            List<OptionBase> opts = new();
            for (int i = 0; i < count; i++)
            {
                opts.Add(new()
                {
                    Index = startIndex + i,
                    PlaceHolder = $"Možnost {(char)(startIndex + i + 65)}",
                    IsCorrect = false
                });
            }
            return opts;
        }   

        [HttpPost]
        public async Task<IActionResult> Preview([FromBody] PreviewRequest model)
        {
            if (model.OptionCount == 0) { return BadRequest("Počet nemůže být nula."); }
            try
            {
                QuestionType? questType = await _ctx.QuestionTypes.FindAsync(model.QuestionTypeId);

                if (questType == null) { return NotFound("Typ otázky nebyl nalezen, nemohu vygenerovat možnosti"); }

                if (questType.Name == "Uzavřená otázka")
                {
                    return PartialView(partialText + "_SelectTextPreview.cshtml", model.OptionCount);
                }
                else if (questType.Name == "Uzavřená otázka s obrázky")
                {
                    return PartialView(partialImage + "_SelectImagePreview.cshtml", model.OptionCount);
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

        [HttpPost]
        [Route("api/QuestionView/AnswerOption")]
        public async Task<IActionResult> AnswerOption([FromBody] OptionRequest model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest("Neplatná data.");
            }

            if (model.QuestionCount == 0) { return BadRequest("Počet nemůže být nula."); }
            try
            {
                QuestionType? questType = await _ctx.QuestionTypes.FindAsync(model.QuestionTypeId);

                if (questType == null) { return NotFound("Typ otázky nebyl nalezen, nemohu vygenerovat možnosti"); }

                // For future responsive use
                Response.Headers.Append("X-Question-Type", WebUtility.UrlEncode(questType.Name));

                List<OptionBase> opts = new();
                for (int i = 0; i < model.QuestionCount; i++)
                {
                    opts.Add(new()
                    {
                        Index = model.CurrentCount + i,
                        PlaceHolder = $"Možnost {(char)(model.CurrentCount + i + 65)}",
                        IsCorrect = false
                    });
                }

                if (questType.Name == "Uzavřená otázka")
                {
                    return PartialView(partialText + "_SelectTextAnswerOption.cshtml", opts);
                }
                else if (questType.Name == "Uzavřená otázka s obrázky")
                {
                    return PartialView(partialImage + "_SelectImageAnswerOption.cshtml", opts);
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


        [HttpPost]
        [Route("api/QuestionView/PreviewOptions")]
        public async Task<IActionResult> PreviewOptions([FromBody] OptionRequest model)
        {
            if (model.QuestionCount == 0) { return BadRequest("Počet nemůže být nula."); }

            try
            {
                QuestionType? questType = await _ctx.QuestionTypes.FindAsync(model.QuestionTypeId);

                if (questType == null) { return NotFound("Typ otázky nebyl nalezen, nemohu vygenerovat možnosti"); }

                List<OptionBase> opts = GenerateOptions(model.CurrentCount, model.QuestionCount);

                if (questType.Name == "Uzavřená otázka")
                {
                    return PartialView(partialText + "_SelectTextPreviewOption.cshtml", opts);
                }
                else if (questType.Name == "Uzavřená otázka s obrázky")
                {
                    return PartialView(partialImage + "_SelectImagePreviewOption.cshtml", opts);
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
    }

    public class OptionRequest
    {
        public int QuestionTypeId { get; set; }
        public int CurrentCount { get; set; }
        public int QuestionCount { get; set; }
    }

    public class PreviewRequest
    {
        public int QuestionTypeId { get; set; }
        public int OptionCount { get; set; }
    }
}
