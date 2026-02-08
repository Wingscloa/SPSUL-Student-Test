using System.ComponentModel.DataAnnotations;
using SPSUL.Controllers.Attribute;

namespace SPSUL.Models.Display.QuestionModels
{
    public class QuestionCreateDto
    {
        public string Header { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int QuestionTypeId { get; set; }
        public int FieldId { get; set; }
        public required List<QuestionOptionDto> Options { get; set; }
    }
}
