using SPSUL.Models.Data;

namespace SPSUL.Models.Display.Quest
{
    public class QuestIndexVM
    {
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
        public int? FieldId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? CreatorId { get; set; }
        public PaginatedList<QuestionRow> Questions { get; set; }
        public List<StudentField> Fields { get; set; }
        public List<Teacher> Teachers { get; set; }
        public List<QuestionType> QuestionTypes { get; set; }
    }
}
