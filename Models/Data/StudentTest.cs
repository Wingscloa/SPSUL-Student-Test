namespace SPSUL.Models.Data
{
    public class StudentTest
    {
        public int StudentId { get; set; }
        public int TestId { get; set; }
        public string LoginId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public string ResultSnapshot { get; set; }
        public virtual Student Student { get; set; }
        public virtual Test Test { get; set; }
    }
}
