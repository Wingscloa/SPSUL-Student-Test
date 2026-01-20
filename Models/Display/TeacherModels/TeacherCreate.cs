using Microsoft.AspNetCore.Mvc;

namespace SPSUL.Models.Display.TeacherModels
{
    public class TeacherCreate : TeacherBase
    {
        public required string Password { get; set; }
    }
}
