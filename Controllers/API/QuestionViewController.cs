using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SPSUL.Models;
using SPSUL.Models.Data;
using SPSUL.Models.Display.QuestionForm;
using SPSUL.Models.Types;
using System.Net;

namespace SPSUL.Controllers.API
{
    /// <summary>
    /// API controller pro dynamické generování náhledu možností otázky při její tvorbě.
    ///
    /// Jak funguje náhled:
    ///   Učitel při tvorbě otázky vybere počet možností a typ otázky.
    ///   JavaScript zavolá POST /api/QuestionView/Preview s těmito hodnotami.
    ///   Server vygeneruje HTML náhled (partial view) s prázdnými polími pro možnosti.
    ///   JavaScript HTML vloží do stránky – učitel může okamžitě vidět, jak otázka bude vypadat.
    ///
    /// Typy náhledů:
    ///   - SelectText  – textové možnosti (A, B, C...)
    ///   - SelectImage – možnosti s obrázkem (nahrávání přes Base64)
    /// </summary>
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
                    PlaceHolder = $"{(char)(startIndex + i + 65)}",
                    IsCorrect = false
                });
            }
            return opts;
        }   

        [HttpPost]
        public async Task<IActionResult> Preview([FromBody] PreviewRequest? model)
        {
            if (model == null) { return BadRequest("Neplatná data."); }
            if (model.OptionCount == 0) { return BadRequest("Počet nemůže být nula."); }
            try
            {
                QuestionType? questType = await _ctx.QuestionTypes.FindAsync(model.QuestionTypeId);

                if (questType == null) { return NotFound("Typ otázky nebyl nalezen, nemohu vygenerovat možnosti"); }

                if (questType.Name == QuestionTypesEnum.SelectText)
                {
                    return PartialView(partialText + "_SelectTextPreview.cshtml", model.OptionCount);
                }
                else if (questType.Name == QuestionTypesEnum.SelectImage)
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
        public async Task<IActionResult> AnswerOption([FromBody] OptionRequest? model)
        {
            if (model == null || !ModelState.IsValid)
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

                if (questType.Name == QuestionTypesEnum.SelectText)
                {
                    return PartialView(partialText + "_SelectTextAnswerOption.cshtml", opts);
                }
                else if (questType.Name == QuestionTypesEnum.SelectImage)
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
        public async Task<IActionResult> PreviewOptions([FromBody] OptionRequest? model)
        {
            if (model == null) { return BadRequest("Neplatná data."); }
            if (model.QuestionCount == 0) { return BadRequest("Počet nemůže být nula."); }

            try
            {
                QuestionType? questType = await _ctx.QuestionTypes.FindAsync(model.QuestionTypeId);

                if (questType == null) { return NotFound("Typ otázky nebyl nalezen, nemohu vygenerovat možnosti"); }

                List<OptionBase> opts = GenerateOptions(model.CurrentCount, model.QuestionCount);

                if (questType.Name == QuestionTypesEnum.SelectText)
                {
                    return PartialView(partialText + "_SelectTextPreviewOption.cshtml", opts);
                }
                else if (questType.Name == QuestionTypesEnum.SelectImage)
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
