using SPSUL.Models.Data;
using SPSUL.Models.Display.QuestionForm;

namespace SPSUL.Models.Display.QuestionModels
{
    public class QuestEditVM
    {
        public required Question Question { get; set; }
        public required List<QuestionType> QuestionTypes { get; set; }
        public required List<StudentField> StudentFields { get; set; }
        public required List<OptionEdit> Options { get; set; }
    }
}
