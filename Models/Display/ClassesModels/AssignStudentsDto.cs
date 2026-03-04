namespace SPSUL.Models.Display.ClassesModels
{
    public class AssignStudentsDto
    {
        public int ClassId { get; set; }
        public List<int> StudentIds { get; set; } = [];
    }
}
