namespace SPSUL.Models.Display.QuestionConfig
{
    public class OptionBase
    {
        public int Index { get; set; }
        public required string PlaceHolder { get; set; }
        public bool IsCorrect { get; set; }
    }
}
