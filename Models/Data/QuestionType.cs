namespace SPSUL.Models.Data
{
    public class QuestionType
    {
        public int QuestionTypeId { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
