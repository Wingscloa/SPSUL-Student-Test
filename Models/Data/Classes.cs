namespace SPSUL.Models.Data
{
    public class Classes
    {
        public int ClassesId { get; set; }
        public required string Name { get; set; }
        public DateTime StartFrom { get; set; }
        public DateTime EndTo { get; set; }
        public bool IsActive { get; set; }
        public ICollection<ClassesStudent> ClassesStudents { get; set; }
    }
}
