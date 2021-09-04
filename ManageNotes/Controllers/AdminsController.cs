using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPPlus.Core.Extensions.Attributes;
using ManageNotes.ActionResult;
using ManageNotes.Data;
using ManageNotes.Models;
using ManageNotes.Services;
using ManageNotes.Utils;
using ManageNotes.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageNotes.Controllers
{
    [Authorize(Roles = KeyValues.OwnerOrAdmin)]
    public class AdminsController : Controller
    {
        private ApplicationContext _applicationContext;
        private UserServices _userServices;

        public AdminsController(ApplicationContext applicationContext, UserServices userServices)
        {
            _applicationContext = applicationContext;
            _userServices = userServices;
        }

        public IActionResult IndexAdmins()
        {
            return View();
        }

        public IActionResult ShowUsers()
        {
            return View();
        }

        //[HttpPost]
        public async Task<IActionResult> GetUsersOrAdmin(int start, int length, string type)
        {
            var filter = Request.Query["search[value]"].ToString();
            TypeAccountEnum t=TypeAccountEnum.Member;
            if (type=="admin")
            {
                if (!User.IsInRole(TypeAccountEnum.Owner.GetNumberWithString()))
                {
                    RedirectToAction("AccessDenied", "Account");
                }
                t = TypeAccountEnum.Admin;
            }
            var users =await _userServices.GetAllUsers(t);

            var totalUsersCount = users.Count;
            var filteredUsersCount = totalUsersCount;

            var list = users.Select(x => new UserViewModel()
            {
                Id = x.Id,
                UserName = x.UserName,
                Number = users.IndexOf(x) + 1
            }).ToList();

            if (!String.IsNullOrEmpty(filter))
            {
                list = list.Where(x => x.UserName.Contains(filter) || x.Number.ToString() == filter)
                    .ToList();
                filteredUsersCount = list.Count;
            }

            list = list.Skip(start).Take(length).ToList();

            return Json(new {data=list, recordsTotal=totalUsersCount, recordsFiltered=filteredUsersCount});
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUser(int id)
        {
            var result =await _userServices.RemoveUser(id);
            if (!result)
            {
                return Json(new {state = false, message = "حساب کاربری با این مشخصات یافت نشد."});
            }

            var rows =await _applicationContext.SaveChangesAsync();

            return Json(new {state = rows > 0});
        }


        public async Task<IActionResult> ExportUsersOrAdmins(string type)
        {
            var fileName = "members";
            TypeAccountEnum t=TypeAccountEnum.Member;
            if (type=="admin")
            {
                if (!User.IsInRole(TypeAccountEnum.Owner.GetNumberWithString()))
                {
                    RedirectToAction("AccessDenied", "Account");
                }
                t = TypeAccountEnum.Admin;
                fileName = "admins";
            }
            var list =await _userServices.GetAllUsers(t);
            var result = new List<UserTest>();
            foreach (var user in list)
            {
                result.Add(user.Adapt<UserTest>());
            }

            if (result.Count==0)
            {
                return new EmptyResult();
            }

            var bytes =await ExcelWriter.GetExcelBytesAsync(result);
            
            return new ExcelResult(bytes,fileName);
        }
        private class UserTest
        {
            [ExcelTableColumn("نام کاربری")]
            public string UserName { get; set; }
            [ExcelTableColumn("نقش")]
            public TypeAccountEnum Role { get; set; }
            [ExcelTableColumn("سریال")]
            public string SerialNo { get; set; }
        }

        [Authorize(Roles = KeyValues.Owner)]
        public IActionResult ShowAdmins()
        {

            return View();
        }
    }
}