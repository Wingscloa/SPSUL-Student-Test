using Microsoft.AspNetCore.Mvc;

namespace SPSUL.Controllers
{
    public enum NotificationType
    {
        Info, // 5095ff
        Warning, // ffb250
        Error, // ff6350
        Success // 9bc576
    }
    public static class ControllerExtensions
    {
        public static void Alert(this Controller controller, string message, NotificationType type)
        {
            controller.TempData["Notification"] = new { message, type };
        }
    }
}
