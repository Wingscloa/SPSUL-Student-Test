namespace SPSUL.Models.Display.QuestionModels
{
    public class QuestionUpdateDto : QuestionCreateDto
    {
        public int QuestionId { get; set; }
        public bool IsActive { get; set; }
    }
}
