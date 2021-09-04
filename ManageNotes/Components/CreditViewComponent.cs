using System.Threading.Tasks;
using ManageNotes.Services;
using ManageNotes.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace ManageNotes.Components
{
    public class CreditViewComponent:ViewComponent
    {
        private UserServices _userServices;

        public CreditViewComponent(UserServices userServices)
        {
            _userServices = userServices;
        }

        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            var user =await _userServices.FindAsync(userId);
            var model = new CreditModel()
            {
                Amount = user.Credit
            };
            return View("_default", model);
        }
    }
}