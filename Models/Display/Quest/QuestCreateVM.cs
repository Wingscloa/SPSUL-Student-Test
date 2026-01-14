using SPSUL.Models.Data;

namespace SPSUL.Models.Display.Quest
{
    public class QuestCreateVM
    {
        public List<QuestionType> QuestionTypes { get; set; }
        public List<StudentField> StudentFields { get; set; }
    }
}
