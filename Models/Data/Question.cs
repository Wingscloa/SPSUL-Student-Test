namespace SPSUL.Models.Data
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int QuestionTypeId { get; set; }
        public int StudentFieldId { get; set; }
        public required string Header { get; set; }
        public required string Description { get; set; }
        public virtual QuestionType QuestionType { get; set; }
        public virtual StudentField StudentField { get; set; }
        public virtual ICollection<QuestionOption> QuestionOptions { get; set; }
    }
}
