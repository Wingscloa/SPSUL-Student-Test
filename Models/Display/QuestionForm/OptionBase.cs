namespace SPSUL.Models.Display.QuestionForm
{
    public class OptionBase
    {
        public int Index { get; set; }
        public required string PlaceHolder { get; set; }
        public bool IsCorrect { get; set; }
    }
}
