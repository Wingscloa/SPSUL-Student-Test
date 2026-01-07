namespace SPSUL.Models.Data
{
    public class Classes
    {
        public int ClassesId { get; set; }
        public required string Name { get; set; }
        public int StartFrom { get; set; }
        public int EndTo { get; set; }
        public bool IsActive { get; set; }
        public ICollection<ClassesStudent> ClassesStudents { get; set; }
        public ICollection<ClassesFields> ClassesFields { get; set; }
    }
}
