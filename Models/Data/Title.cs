namespace SPSUL.Models.Data
{
    public class Title
    {
        public int TitleId { get; set; }
        public required string Shortcut { get; set; }
        public required string Name { get; set; }
        public ICollection<TeacherTitle> TeacherTitles { get; set; }
    }
}
