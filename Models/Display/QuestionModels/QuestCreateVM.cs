using SPSUL.Models.Display.QuestionForm;
using SPSUL.Models.Data;

namespace SPSUL.Models.Display.QuestionModels
{
    public class QuestCreateVM
    {
        public required List<QuestionType> QuestionTypes { get; set; }
        public required List<StudentField> StudentFields { get; set; }
        public required List<OptionBase> OptionBases { get; set; }
        public required string SelectedQuestionName { get; set; }
    }
}