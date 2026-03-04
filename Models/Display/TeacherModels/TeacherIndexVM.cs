using SPSUL.Models.Data;

namespace SPSUL.Models.Display.TeacherModels
{
    public class TeacherIndexVM
    {
        public List<Teacher> Teachers { get; set; } = [];
        public List<Role> Roles { get; set; } = [];
        public List<Title> Titles { get; set; } = [];

        // Filter state
        public string? Name { get; set; }
        public int? RoleId { get; set; }
        public int? TitleId { get; set; }
        public bool? Active { get; set; }
    }
}
