namespace SPSUL.Models.Display.Quest
{
    public class QuestionRow
    {
        public int QuestionId { get; set; }
        public required string Header { get; set; }
        public required string Description { get; set; }
        public required string CreatorName { get; set; }
        public int OptionCount { get; set; }
        public required string QuestionTypeName { get; set; }
        public required string FieldName { get; set; }
        public bool IsActive { get; set; }
    }
}
