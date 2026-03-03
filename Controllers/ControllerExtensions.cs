using Microsoft.AspNetCore.Mvc;

namespace SPSUL.Controllers
{
    /// <summary>Typy notifikací zobrazovaných uživateli (toastr).</summary>
    public enum NotificationType
    {
        Info,    // modrá - informace
        Warning, // žlutá - varování
        Error,   // červená - chyba
        Success  // zelená - úspěch
    }

    /// <summary>
    /// Rozšíření třídy Controller o pomocné metody.
    ///
    /// Jak funguje notifikace:
    ///   Vloží data do TempData["Notification"] (které přežije redirect).
    ///   Partial view _NotificationHandler.cshtml tato data přečte a zobrazí toastr alert.
    ///
    /// Použití:
    ///   this.Alert("Uloženo!", NotificationType.Success);
    ///   return RedirectToAction("Index");
    /// </summary>
    public static class ControllerExtensions
    {
        public static void Alert(this Controller controller, string message, NotificationType type)
        {
            controller.TempData["NotificationMessage"] = message;
            controller.TempData["NotificationType"] = type.ToString().ToLower();
        }
    }
}
