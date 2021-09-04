using Microsoft.AspNetCore.Mvc;

namespace ManageNotes.Controllers
{
    public class AccountController : Controller
    {
        // GET
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}