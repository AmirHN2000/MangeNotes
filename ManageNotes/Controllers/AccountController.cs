using Microsoft.AspNetCore.Mvc;

namespace ManageNotes.Controllers
{
    public class AccountController : Controller
    {
        //get
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
