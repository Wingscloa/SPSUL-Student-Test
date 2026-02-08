using SPSUL.Models.Data;

namespace SPSUL.Models.Display.StudentModels
{
    public class StudentIndexVM
    {
        public List<Student> Students { get; set; } = [];
        public List<Classes> Classes { get; set; } = [];
        public List<StudentField> Fields { get; set; } = [];

        // Filter state
        public string? Name { get; set; }
        public int? ClassId { get; set; }
        public int? FieldId { get; set; }
        public bool? Active { get; set; }
    }
}
