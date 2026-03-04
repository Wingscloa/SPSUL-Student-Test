namespace SPSUL.Models.Display.QuestionModels
{
    public class QuestionFilter
    {
        public string? SearchFilter { get; set; }
        public bool? ActiveFilter { get; set; }
        public int? CreatorId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? FieldId { get; set; }
    }
}
