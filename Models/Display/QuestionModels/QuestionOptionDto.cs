namespace SPSUL.Models.Display.QuestionModels
{
    public class QuestionOptionDto
    {
        public string Text { get; set; } = string.Empty;
        public string? ImageBase64 { get; set; }
        public bool IsCorrect { get; set; }
    }
}
