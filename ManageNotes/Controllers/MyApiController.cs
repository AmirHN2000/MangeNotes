using System.Linq;
using System.Threading.Tasks;
using ManageNotes.Data;
using ManageNotes.Services;
using ManageNotes.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManageNotes.Controllers
{
    [ApiController]
    [Route("/myapi")]
    public class MyApiController : Controller
    {
        private ApplicationContext _applicationContext;
        private UserServices _userServices;

        public MyApiController(ApplicationContext applicationContext, UserServices userServices)
        {
            _applicationContext = applicationContext;
            _userServices = userServices;
        }

        /// <summary>
        /// get user information with username
        /// </summary>
        /// <param name="name"></param>
        /// <returns>return name, id, role and create date of user</returns>
        [HttpGet]
        public async Task<IActionResult> Get(string name)
        {
            var user =await _userServices.GetUserWithUserNameAsync(name);
            return Ok(new
            {
                id=user.Id, name=user.UserName, role=user.Role,
                createDate=user.CreatedDate
            });
        }

        /// <summary>
        /// update user support username, id and role of user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>return object send to post api</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserViewModel model)
        {
            var user =await _userServices.GetUserWithUserNameAsync(model.UserName);
            if (user is null)
            {
                return NotFound("User Not Fond.");
            }

            user.Role = model.Role;
            var rows=await _applicationContext.SaveChangesAsync();
            if (rows>0)
            {
                return Ok(model);
            }

            return Problem("Error");
        }
    }
}