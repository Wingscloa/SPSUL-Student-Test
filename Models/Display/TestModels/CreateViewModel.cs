using SPSUL.Models.Data;

namespace SPSUL.Models.Display.TestModels
{
    public class CreateViewModel
    {
        public List<StudentField> Fields { get; set; } = new();
        public List<Student> Students { get; set; } = new();
        public List<Classes> Classes { get; set; } = new();
        public List<Question> Questions { get; set; } = new();
        public List<QuestionType> Types { get; set; } = new();
    }
}
