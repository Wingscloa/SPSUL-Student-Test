namespace SPSUL.Models.Display.QuestionForm
{
    public class OptionEdit
    {
        public required string Text { get; set; }
        public required string ImageKey { get; set; }
        public bool IsCorrect { get; set; }

        public OptionEdit() { }

        public OptionEdit(string text, string imageBase64, bool isCorrect)
        {
            Text = text;
            ImageKey = imageBase64;
            IsCorrect = isCorrect;
        }
    }
}
