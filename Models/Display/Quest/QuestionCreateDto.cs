namespace SPSUL.Models.Display.Quest
{
    public class QuestionCreateDto
    {
        public string Header { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int QuestionTypeId { get; set; }
        public int FieldId { get; set; }
        public List<QuestionOptionDto> Options { get; set; } = new();
    }
}
