using SPSUL.Models.Data;

namespace SPSUL.Models.Display.QuestionModels
{
    public class QuestEditVM
    {
        public Question Question { get; set; }
        public required List<QuestionType> QuestionTypes { get; set; }
        public required List<StudentField> StudentFields { get; set; }
    }
}
