namespace SPSUL.Models.Display.TeacherModels
{
    public class TeacherBase
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string NickName { get; set; }
        public List<int>? TitleIds { get; set; }
        public List<int>? RoleIds { get; set; }

    }
}
