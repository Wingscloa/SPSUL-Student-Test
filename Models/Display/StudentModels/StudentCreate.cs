namespace SPSUL.Models.Display.StudentModels
{
    public class StudentCreate
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public int ClassesId { get; set; }
    }
}
