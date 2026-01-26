using SPSUL.Models.Display.QuestionConfig;
using SPSUL.Models.Data;

namespace SPSUL.Models.Display.QuestionModels
{
    public class QuestCreateVM
    {
        public required List<QuestionType> QuestionTypes { get; set; }
        public required List<StudentField> StudentFields { get; set; }
        public required List<OptionBase> OptionBases { get; set; }
    }
}