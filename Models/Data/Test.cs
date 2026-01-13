namespace SPSUL.Models.Data
{
    public class Test
    {
        public int TestId { get; set; }
        public required string Name { get; set; }
        public int CreatorId { get; set; }
        public int StudentFieldId { get; set; }
        public required string QuestionSnapshot { get; set; }
        public bool IsActive { get; set; }
        public virtual StudentField StudentField { get; set; }
        public virtual Teacher Creator { get; set; }
        public virtual ICollection<StudentTest> StudentTests { get; set; }
    }
}