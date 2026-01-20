namespace SPSUL.Models.Display.TeacherModels
{
    public class TeacherFilter
    {
        public string? SearchFilter { get; set; }
        public List<int>? TitleFilterIds { get; set; }
        public List<int>? RoleFilterIds { get; set; }
        public bool? ActiveFilter { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 13;
    }
}
