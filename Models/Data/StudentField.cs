namespace SPSUL.Models.Data
{
    public class StudentField
    {
        public int StudentFieldId { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
        public ICollection<ClassesFields> ClassesFields { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
