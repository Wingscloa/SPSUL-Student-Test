namespace SPSUL.Models.Display.StudentModels
{
    public class StudentUpdate
    {
        public int StudentId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public List<int>? ClassesIds { get; set; }
        public bool IsActive { get; set; }
    }
}
