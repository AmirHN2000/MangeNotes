using Microsoft.AspNetCore.Mvc;

namespace ManageNotes.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
