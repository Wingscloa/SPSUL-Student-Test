using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;
using Microsoft.EntityFrameworkCore; 
using SPSUL.Models.Display.QuestionModels;
using SPSUL.Models.Display;
using SPSUL.Models.Display.QuestionForm;

namespace SPSUL.Controllers
{
    [LoginRequired]
    public class QuestionController : Controller
    {

        private readonly SpsulContext _ctx;
        private readonly AzureBlobService _blobService;
        private readonly SharedService _sharedService;
        private readonly ILogger<QuestionController> _logger;
        public QuestionController(SpsulContext ctx, AzureBlobService blobService, SharedService sharedService, ILogger<QuestionController> logger)
        {
            _ctx = ctx;
            _blobService = blobService;
            _sharedService = sharedService;
            _logger = logger;
        }
        
        public async Task<IActionResult> Index(string? Name,bool? IsActive, int? FieldId, int? QuestionTypeId, int? CreatorId, int pageNumber = 1, int pageSize = 13)
        {
            try
            {
                bool offFilter = true;
                List<int> questionIds = new List<int>();
                if(Name != null || IsActive != null || FieldId != null || QuestionTypeId != null || CreatorId != null)
                {
                    pageNumber = 1;
                    questionIds = await _ctx.Questions
                    .Include(q => q.Creator)
                    .Include(q => q.QuestionType)
                    .Include(q => q.Field)
                    .Include(q => q.QuestionOptions)
                    .Where(e =>
                        (Name == null || e.Header.Contains(Name)) &&
                        (IsActive == null || e.IsActive == IsActive) &&
                        (FieldId == null || e.FieldId == FieldId) &&
                        (QuestionTypeId == null || e.QuestionTypeId == QuestionTypeId) &&
                        (CreatorId == null || e.CreatorId == CreatorId)
                        )
                    .Select(q => q.QuestionId)
                    .ToListAsync();
                    offFilter = false;
                }

                List<QuestionRow> query = await _ctx.QuestionRow.Where(e =>
                    (offFilter || questionIds.Contains(e.QuestionId))).ToListAsync();

                List<QuestionRow> rows = query
                    .OrderByDescending(e => e.QuestionId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                int count = query.Count;

                QuestIndexVM model = new()
                {
                    Questions = new PaginatedList<QuestionRow>(rows, count, pageNumber, pageSize),
                    Fields = await _ctx.StudentFields.Where(e => e.IsActive == true).ToListAsync(),
                    QuestionTypes = await _ctx.QuestionTypes.Where(e => e.IsActive == true).ToListAsync(),
                    Teachers = await _ctx.Teachers.Include(e => e.Titles).ThenInclude(e => e.Title).ToListAsync(),
                    CreatorId = CreatorId,
                    FieldId = FieldId,
                    IsActive = IsActive,
                    Name = Name,
                    QuestionTypeId = QuestionTypeId
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při načítání otázek pro index.");
                return View("Error");
            }
        }
        
        public async Task<IActionResult> Create()
        {
            try
            {
                QuestCreateVM model = new()
                {
                    QuestionTypes = await _ctx.QuestionTypes.Where(e => e.IsActive == true).ToListAsync(),
                    StudentFields = await _ctx.StudentFields.Where(e => e.IsActive == true).ToListAsync(),
                    OptionBases = new List<OptionBase>
                    {
                        new OptionBase { Index = 0, PlaceHolder = "Možnost A" },
                        new OptionBase { Index = 1, PlaceHolder = "Možnost B" },
                        new OptionBase { Index = 2, PlaceHolder = "Možnost C" },
                        new OptionBase { Index = 3, PlaceHolder = "Možnost D" }
                    },
                    SelectedQuestionName = "Uzavřená otázka",
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
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
                StudentFields = await _ctx.StudentFields.Where(e => e.IsActive == true).ToListAsync(),
                PreviewEdit = new() { Description = question.Description, 
                    Header = question.Header, 
                    Options = question.QuestionOptions.Select(e => e).ToList() 
                },
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<int> Ids)
        {
            using var transaction = await _ctx.Database.BeginTransactionAsync();
            
            try
            {
                var questions = await _ctx.Questions
                    .Include(q => q.QuestionOptions)
                    .Where(q => Ids.Contains(q.QuestionId))
                    .ToListAsync();

                if (questions.Count == 0)
                {
                    return NotFound(new { message = "Žádné otázky nebyly nalezeny." });
                }

                // Shromáždění všech ImageKeys k smazání ze všech otázek
                var imagesToDelete = questions
                    .SelectMany(q => q.QuestionOptions)
                    .Where(o => !string.IsNullOrWhiteSpace(o.ImageKey))
                    .Select(o => o.ImageKey)
                    .ToList();

                // Smazání otázek z databáze
                foreach (var question in questions)
                {
                    _ctx.Questions.Remove(question);
                }
                await _ctx.SaveChangesAsync();

                // Potvrzení databázové transakce
                await transaction.CommitAsync();

                // Smazání obrázků z Azure (po úspěšném smazání z DB)
                int deletedImagesCount = 0;
                if (imagesToDelete.Any())
                {
                    deletedImagesCount = await _blobService.DeleteBlobsAsync(imagesToDelete);
                    Console.WriteLine($"Smazáno {deletedImagesCount} obrázků z Azure Storage pro {questions.Count} otázek.");
                }

                return Ok(new 
                { 
                    message = "Otázky byly úspěšně smazány!",
                    deletedQuestionsCount = questions.Count,
                    deletedImagesCount
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new 
                { 
                    message = "Chyba při smazání otázek: " + ex.Message,
                    innerError = ex.InnerException?.Message 
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Activate([FromBody] List<int> Ids)
        {
            try
            {
                var questions = await _ctx.Questions
                    .Where(q => Ids.Contains(q.QuestionId))
                    .ToListAsync();
                if (questions.Count == 0)
                {
                    return NotFound(new { message = "Žádné otázky nebyly nalezeny." });
                }
                foreach (var question in questions)
                {
                    question.IsActive = true;
                }
                await _ctx.SaveChangesAsync();
                return Ok(new { message = "Otázky byly úspěšně aktivovány!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Chyba při aktivaci otázek: " + ex.Message });

            }
        }

        [HttpPut]
        public async Task<IActionResult> Deactivate([FromBody] List<int>Ids)
        {
            try
            {
                var questions = await _ctx.Questions
                    .Where(q => Ids.Contains(q.QuestionId))
                    .ToListAsync();
                if (questions.Count == 0)
                {
                    return NotFound(new { message = "Žádné otázky nebyly nalezeny." });
                }
                foreach (var question in questions)
                {
                    question.IsActive = false;
                }
                await _ctx.SaveChangesAsync();
                return Ok(new { message = "Otázky byly úspěšně deaktivovány!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Chyba při deaktivaci otázek: " + ex.Message });

            }
        }

        [HttpPost]
        [LoginRequired]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionCreateDto dto)
        {
            // Validace pomocí ModelState (Data Annotations z DTO)
            if (!ModelState.IsValid)    
            {
                var errors = ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(e => e.Value!.Errors.Select(err => 
                        string.IsNullOrEmpty(err.ErrorMessage) 
                            ? $"{e.Key}: {err.Exception?.Message}" 
                            : err.ErrorMessage))
                    .ToList();

                return BadRequest(new 
                { 
                    message = "Neplatná vstupní data.",
                    errors
                });
            }

            using var transaction = await _ctx.Database.BeginTransactionAsync();
            var uploadedImageKeys = new List<string>();

            try
            {
                // Vytvoření otázky
                Question question = new Question
                {
                    Header = dto.Header.Trim(),
                    Description = dto.Description.Trim(),
                    QuestionTypeId = dto.QuestionTypeId,
                    FieldId = dto.FieldId,
                    IsActive = true,
                    CreatorId = (int)HttpContext.Items["CurrentUserId"]
                };

                _ctx.Questions.Add(question);
                await _ctx.SaveChangesAsync(); // Získání QuestionId

                // Zpracování odpovědí
                List<QuestionOption> options = new();
                for (int index = 0; index < dto.Options.Count; index++)
                {
                    var optionDto = dto.Options[index];
                    string? imageKey = null;

                    // Nahrání obrázku (pokud existuje)
                    if (!string.IsNullOrWhiteSpace(optionDto.ImageBase64))
                    {
                        try
                        {
                            // Generování unikátního klíče: questionId_index_guid
                            imageKey = $"q{question.QuestionId}_opt{index}_{Guid.NewGuid():N}";
                            
                            IFormFile imageFile = _blobService.ConvertBase64ToIFormFile(optionDto.ImageBase64, imageKey);
                            await _blobService.UploadOptimizedAsync(imageFile, imageKey);
                            
                            uploadedImageKeys.Add(imageKey); // Pro případný rollback
                        }
                        catch (Exception imgEx)
                        {
                            throw new ApplicationException($"Chyba při nahrávání obrázku u možnosti {index + 1}: {imgEx.Message}", imgEx);
                        }
                    }

                    // Vytvoření option entity
                    options.Add(new QuestionOption
                    {
                        QuestionId = question.QuestionId,
                        Text = optionDto.Text?.Trim() ?? string.Empty,
                        ImageKey = imageKey ?? string.Empty,
                        IsCorrect = optionDto.IsCorrect
                    });
                }

                // Uložení options do databáze
                _ctx.QuestionOptions.AddRange(options);
                await _ctx.SaveChangesAsync();

                // Potvrzení transakce
                await transaction.CommitAsync();

                return Ok(new 
                { 
                    message = "Otázka byla úspěšně vytvořena!", 
                    questionId = question.QuestionId,
                    optionsCount = options.Count,
                    imagesCount = uploadedImageKeys.Count
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
        
                // ROLLBACK - smazání nahraných obrázků
                if (uploadedImageKeys.Any())
                {
                    int deleted = await _blobService.DeleteBlobsAsync(uploadedImageKeys);
                    Console.WriteLine($"Rollback: Smazáno {deleted} obrázků");
                }
        
                return BadRequest(new { message = "Chyba při vytváření otázky.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message 
                });
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
            // Validace pomocí ModelState (Data Annotations z DTO)
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(e => e.Value!.Errors.Select(err => 
                        string.IsNullOrEmpty(err.ErrorMessage) 
                            ? $"{e.Key}: {err.Exception?.Message}" 
                            : err.ErrorMessage))
                    .ToList();

                return BadRequest(new 
                { 
                    message = "Neplatná vstupní data.",
                    errors
                });
            }

            using var transaction = await _ctx.Database.BeginTransactionAsync();
            var uploadedImageKeys = new List<string>(); // Nově nahrané obrázky
            var imagesToDelete = new List<string>(); // Staré obrázky ke smazání

            try
            {
                // Načtení existující otázky
                var question = await _ctx.Questions
                    .Include(q => q.QuestionOptions)
                    .FirstOrDefaultAsync(q => q.QuestionId == dto.QuestionId);

                if (question == null)
                {
                    return NotFound(new { message = "Otázka nebyla nalezena." });
                }

                // Aktualizace základních údajů otázky
                question.Header = dto.Header.Trim();
                question.Description = dto.Description.Trim();
                question.QuestionTypeId = dto.QuestionTypeId;
                question.FieldId = dto.FieldId;

                // Získání starých options pro porovnání obrázků
                var oldOptions = question.QuestionOptions.ToList();

                // Smazání starých options z databáze
                _ctx.QuestionOptions.RemoveRange(oldOptions);
                await _ctx.SaveChangesAsync(); // Uložit změny pro získání správného stavu

                // Zpracování nových odpovědí
                List<QuestionOption> newOptions = new();
                for (int index = 0; index < dto.Options.Count; index++)
                {
                    QuestionOptionDto optionDto = dto.Options[index];
                    string? imageKey = null;

                    // Kontrola, zda byla změněna fotka
                    if (!string.IsNullOrWhiteSpace(optionDto.ImageBase64))
                    {
                        try
                        {
                            // Pokud měla stará option na stejném indexu obrázek, označit ke smazání
                            if (index < oldOptions.Count && !string.IsNullOrWhiteSpace(oldOptions[index].ImageKey))
                            {
                                imagesToDelete.Add(oldOptions[index].ImageKey);
                            }

                            // Generování nového unikátního klíče
                            imageKey = $"q{question.QuestionId}_opt{index}_{Guid.NewGuid():N}";
                            
                            IFormFile imageFile = _blobService.ConvertBase64ToIFormFile(optionDto.ImageBase64, imageKey);
                            await _blobService.UploadOptimizedAsync(imageFile, imageKey);
                            
                            uploadedImageKeys.Add(imageKey);
                        }
                        catch (Exception imgEx)
                        {
                            throw new ApplicationException($"Chyba při nahrávání obrázku u možnosti {index + 1}: {imgEx.Message}", imgEx);
                        }
                    }
                    else
                    {
                        // Pokud není nový obrázek, ale stará option měla obrázek, zachovat starý
                        if (index < oldOptions.Count && !string.IsNullOrWhiteSpace(oldOptions[index].ImageKey))
                        {
                            // Pokud ImageBase64 je prázdný, ale ImageKey existuje z původní option
                            // Zachováme starý ImageKey (uživatel fotku nezměnil)
                            if (string.IsNullOrWhiteSpace(optionDto.ImageBase64))
                            {
                                imageKey = oldOptions[index].ImageKey;
                            }
                            else
                            {
                                // ImageKey byl poskytnut - uživatel možná nechtěl měnit
                                imageKey = optionDto.ImageBase64;
                            }
                        }
                    }

                    // Vytvoření nové option entity
                    newOptions.Add(new QuestionOption
                    {
                        QuestionId = question.QuestionId,
                        Text = optionDto.Text?.Trim() ?? string.Empty,
                        ImageKey = imageKey ?? string.Empty,
                        IsCorrect = optionDto.IsCorrect
                    });
                }

                // Kontrola mazání obrázků navíc (pokud uživatel snížil počet možností)
                if (oldOptions.Count > dto.Options.Count)
                {
                    for (int i = dto.Options.Count; i < oldOptions.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(oldOptions[i].ImageKey))
                        {
                            imagesToDelete.Add(oldOptions[i].ImageKey);
                        }
                    }
                }

                // Přidání nových options do databáze
                _ctx.QuestionOptions.AddRange(newOptions);
                await _ctx.SaveChangesAsync();

                // Smazání starých obrázků z Azure (po úspěšném uložení do DB)
                if (imagesToDelete.Any())
                {
                    int deletedCount = await _blobService.DeleteBlobsAsync(imagesToDelete);
                    Console.WriteLine($"Smazáno {deletedCount} starých obrázků z Azure Storage.");
                }

                // Potvrzení transakce
                await transaction.CommitAsync();

                return Ok(new 
                { 
                    message = "Otázka byla úspěšně aktualizována!",
                    questionId = question.QuestionId,
                    optionsCount = newOptions.Count,
                    newImagesCount = uploadedImageKeys.Count,
                    deletedImagesCount = imagesToDelete.Count
                });
            }
            catch (Exception ex)
            {
                // Rollback databázové transakce
                await transaction.RollbackAsync();

                // ROLLBACK - smazání nově nahraných obrázků
                if (uploadedImageKeys.Any())
                {
                    int deleted = await _blobService.DeleteBlobsAsync(uploadedImageKeys);
                    Console.WriteLine($"Rollback: Smazáno {deleted} nově nahraných obrázků");
                }

                return BadRequest(new 
                { 
                    message = "Chyba při aktualizaci otázky.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message 
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            using var transaction = await _ctx.Database.BeginTransactionAsync();
            
            try
            {
                Question? question = await _ctx.Questions
                    .Include(q => q.QuestionOptions)
                    .FirstOrDefaultAsync(q => q.QuestionId == id);
                
                if (question == null)
                {
                    return NotFound(new { message = "Otázka nebyla nalezena." });
                }

                // Shromáždění všech ImageKeys k smazání
                var imagesToDelete = question.QuestionOptions
                    .Where(o => !string.IsNullOrWhiteSpace(o.ImageKey))
                    .Select(o => o.ImageKey)
                    .ToList();

                // Smazání otázky z databáze (včetně QuestionOptions díky cascade delete)
                _ctx.Questions.Remove(question);
                await _ctx.SaveChangesAsync();

                // Potvrzení databázové transakce
                await transaction.CommitAsync();

                // Smazání obrázků z Azure (po úspěšném smazání z DB)
                int deletedImagesCount = 0;
                if (imagesToDelete.Any())
                {
                    deletedImagesCount = await _blobService.DeleteBlobsAsync(imagesToDelete);
                    Console.WriteLine($"Smazáno {deletedImagesCount} obrázků z Azure Storage pro otázku {id}.");
                }

                return Ok(new 
                { 
                    message = "Otázka byla úspěšně smazána!",
                    questionId = id,
                    deletedImagesCount
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new 
                { 
                    message = "Chyba při mazání otázky: " + ex.Message,
                    innerError = ex.InnerException?.Message 
                });
            }
        }
    }
}
