namespace SPSUL.Models.Display.Quest
{
    public class QuestionUpdateDto : QuestionCreateDto
    {
        public int QuestionId { get; set; }
        public bool IsActive { get; set; }
    }
}
