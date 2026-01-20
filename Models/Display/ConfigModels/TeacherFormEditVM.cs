using SPSUL.Models.Data;

namespace SPSUL.Models.Display.ConfigModels
{
    public class TeacherFormEditVM
    {
        public Teacher Teacher { get; set; } 
        public List<Title> Titles { get; set; }
        public List<Role> Roles { get; set; }
    }
}
