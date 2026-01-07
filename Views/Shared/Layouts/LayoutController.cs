using Microsoft.AspNetCore.Mvc;
using SPSUL.Models.Data;
namespace SPSUL.Views.Shared.Layouts
{
    public class TeacherLayout
    {
        public Teacher Teacher { get; set; }
        public List<Role> Roles { get; set; }
    }
    public class LayoutController : Controller
    {
        public TeacherLayout TeacherLayout { get; set; }
        public LayoutController()
        {
            var x = HttpContext.Session.GetInt32("TeacherId");
        }
    }
}
