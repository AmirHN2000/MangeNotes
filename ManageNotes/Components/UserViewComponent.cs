using System.Threading.Tasks;
using ManageNotes.Data;
using ManageNotes.Services;
using ManageNotes.ViewModel;
using Microsoft.AspNetCore.Mvc;
using ManageNotes.Utils;

namespace ManageNotes.Components
{
    public class UserViewComponent:ViewComponent
    {
        private ApplicationContext _applicationContext;
        private UserServices _userServices;

        public UserViewComponent(ApplicationContext applicationContext, UserServices userServices)
        {
            _applicationContext = applicationContext;
            _userServices = userServices;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (UserClaimsPrincipal.Identity.IsAuthenticated)
            {
                var id = UserClaimsPrincipal.GetId();
                var user=await _userServices.FindAsync(id);
                var model = new UserViewModel()
                {       
                    Id = user.Id,
                    UserName = user.UserName,
                    Role = user.Role
                };
                return View("_default", model);
            }

            return null;
        }
    }
}