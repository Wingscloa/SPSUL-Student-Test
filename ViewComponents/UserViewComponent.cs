using Microsoft.AspNetCore.Mvc;
using SPSUL.Models;
using SPSUL.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SPSUL.ViewComponents
{
    /// <summary>
    /// ViewComponent zobrazující jméno přihlášeného učitele v navbaru (vpravo nahoře).
    ///
    /// Vyvolání v layoutu:
    ///   @await Component.InvokeAsync("User")
    /// </summary>
    public class UserViewComponent : ViewComponent
    {
        private readonly SharedService _sharedService;
        public UserViewComponent(SharedService sharedService)
        {
            _sharedService = sharedService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            string? name = await _sharedService.GetNameAsync();
            return View("Default", name);
        }
    }
}