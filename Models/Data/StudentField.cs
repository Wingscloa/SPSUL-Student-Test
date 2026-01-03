namespace SPSUL.Models.Data
{
    public class StudentField
    {
        public int StudentFieldId { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
