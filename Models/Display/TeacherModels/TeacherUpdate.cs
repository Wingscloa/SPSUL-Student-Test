namespace SPSUL.Models.Display.TeacherModels
{
    public class TeacherUpdate : TeacherBase
    {
        public string? Password { get; set; }
        public int TeacherId { get; set; }
        public bool IsActive { get; set; }
    }
}