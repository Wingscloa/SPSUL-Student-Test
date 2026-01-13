using SPSUL.Models.Data;

namespace SPSUL.Models.Display.Test
{
    public class CreateViewModel
    {
        public List<StudentField> StudentFields { get; set; }
        public List<Student> Students { get; set; }
        public List<Classes> Classes { get; set; }
        public List<SPSUL.Models.Data.Question> Questions { get; set; }
    }
}
