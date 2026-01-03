namespace SPSUL.Models.Data
{
    public class QuestionOption
    {
        public int QuestionOptionId { get; set; }
        public int QuestionId { get; set; }
        public required string ImageBase64 { get; set; }
        public required string Text { get; set; }
        public bool IsCorrect { get; set; }
        public virtual Question Question { get; set; }
    }
}
