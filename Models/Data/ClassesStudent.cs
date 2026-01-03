namespace SPSUL.Models.Data
{
    public class ClassesStudent
    {
        public int ClassStudentId { get; set; }
        public int ClassesId { get; set; }
        public int StudentId { get; set; }
        public virtual Classes Classes { get; set; }
        public virtual Student Student { get; set; }
    }
}
