using SPSUL.Models.Data;

namespace SPSUL.Models.Display.ClassesModels
{
    public class ClassesIndexVM
    {
        public List<Classes> Classes { get; set; } = [];
        public List<StudentField> Fields { get; set; } = [];
        public List<Student> Students { get; set; } = [];

        // Filter state
        public string? Name { get; set; }
        public int? FieldId { get; set; }
        public bool? Active { get; set; }
    }
}
