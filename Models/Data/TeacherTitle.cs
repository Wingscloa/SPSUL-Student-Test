namespace SPSUL.Models.Data
{
    public class TeacherTitle
    {
        public int TeacherId { get; set; }
        public int TitleId { get; set; }
        public Title Title { get; set; }
        public Teacher Teacher { get; set; }
    }
}
