using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ManageNotes.Data;
using ManageNotes.Services;
using ManageNotes.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ManageNotes.Utils;

namespace ManageNotes.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationContext _applicationContext;
        private UserServices _userServices;
        private IWebHostEnvironment _environment;

        public UserController(ApplicationContext applicationContext, UserServices userServices, IWebHostEnvironment environment)
        {
            _applicationContext = applicationContext;
            _userServices = userServices;
            _environment = environment;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LogIn(LogInModel model)
        {
            var url = Request.Query["ReturnUrl"].ToString();
            if (ModelState.IsValid)
            {
                var user =await _userServices.GetUserWithUserNameAsync(model.UserName);
                if (user is null)
                {
                    ModelState.AddModelError("UserName","حسابی با این نام کاربری وجود ندارد.");
                    return View(model);
                }
                
                if (user.PassWord != model.PassWord.GetHash())
                {
                    ModelState.AddModelError("PassWord","رمز عبور اشتباه است.");
                    return View(model);
                }

                var model2 = user.Adapt<UserViewModel>();
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    model2.GetPrincipal());

                /*if (!String.IsNullOrEmpty(url))
                {
                    var splited = url.SplitUrl();
                    return RedirectToAction(splited.actionName, splited.controllerName);
                }*/

                if (user.Role==TypeAccountEnum.Admin || user.Role==TypeAccountEnum.Owner)
                {
                    return RedirectToAction("IndexAdmins", "Admins");
                }
                
                return RedirectToAction("NoteIndex", "Note");
            }
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        
        [AllowAnonymous]
        [HttpGet]
        public IActionResult LogUp()
        {
            return View(new LogUpModel());
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LogUp(LogUpModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _userServices.IsExistUserWithUsernameAsync(model.UserName))
                    return Json(new {status = false, message = "نام کاربری تکراری است. یک نام جدید انتخاب کنید."});
                        
                User user = new User
                {
                    UserName = model.UserName,
                    PassWord = model.PassWord.GetHash(),
                };

                if (model.File is not null)
                {
                    var blackList = new[] {".c", ".bat", ".shell", ".cmd"};
                    var t = blackList.Any(x => x == model.File.ContentType);
                    if (t)
                        
                    
                    if (model.File.Length>(3*1024*1024))
                        return Json(new {status=false, message="حجم فایل نباید بیش از 3 مگابایت باشد."});
                    
                    user.Image =await model.File.GetArrayBytesAsync();
                }

                await _userServices.AddUserAsync(user);
                await _applicationContext.SaveChangesAsync();
                var u =await _applicationContext.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);

                u.Role = TypeAccountEnum.Member;
                if (User.IsInRole(TypeAccountEnum.Owner.GetNumberWithString()))
                {
                    u.Role = model.Role;
                }

                var rows=await _applicationContext.SaveChangesAsync();
                if (rows>0)
                {
                    if (User.Identity.IsAuthenticated  && !User.IsInRole(TypeAccountEnum.Member.GetNumberWithString()))
                        return Json(new {status=true, message="حساب کاربری با موفقیت ایجاد شد."});
                    
                    var u2 = u.Adapt<UserViewModel>();
                    var principal = u2.GetPrincipal();
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        principal);
                    return RedirectToAction("NoteIndex", "Note");
                }
            }

            string s="";
            foreach (var value in ModelState.Values)
            foreach (ModelError error in value.Errors)
                s+=error.ErrorMessage;
            
            return Json(new {status=false, message=s});
        }
        
        public async Task<IActionResult> ShowImage(int id)
        {
            var user =await _userServices.FindAsync(id);
            if (user.Image is not null)
            {
                return new FileContentResult(user.Image, "image/jpeg");
            }

            var wwroot = _environment.WebRootPath;
            var path = Path.Combine(wwroot, "image", "UserDefualt.png");
            return new PhysicalFileResult(path, "image/png");
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile(int id=-1)
        {
            var userId = User.GetId();
            if (id!=-1)
            {
                userId = id;
            }
            var user =await _userServices.FindAsync(userId);
            var model = new EditProfileModel()
            {
                UserName =user.UserName,
                Id=user.Id,
                Role = user.Role,
                Check = id
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileModel model)
        {
            if (ModelState.IsValid)
            {
                var user =await _userServices.FindAsync(model.Id);
                var exist =await _userServices.IsExistUserWithUsernameAsync(model.UserName);
                model.Check = user.Id;
                
                if (user.UserName!=model.UserName && exist)
                    return Json(new {status = false, message = "نام کاربری تکراری است. یک نام جدید انتخاب کنید."});
                

                if (model.File is not null)
                {
                    var blacList = new [] {".c", ".bat", ".shell", ".cmd"};
                    var t = blacList.Any(x => x == model.File.ContentType);
                    if (t)
                        return Json(new {status = false, message = "آپلود فایل نامعتبر"});
                    
                    if (model.File.Length>(3*1024*1024))
                        return Json(new {status = false, message = "حجم فایل نباید بیش از 3 مگابایت باشد."});
                    
                    user.Image =await model.File.GetArrayBytesAsync();
                }
                
                user.UserName = model.UserName;
                if (User.IsInRole(TypeAccountEnum.Owner.GetNumberWithString()))
                {
                    user.Role = model.Role;
                }
                
                await _applicationContext.SaveChangesAsync();
                
                return Json(new {status = true, message = "تغییرات روی حساب کاربری اعمال شد."});
            }

            model.Check = 50;
            return Json(new {status = false, message = "خطا در اعمال تغییرات"});
        }

        [HttpGet]
        [Route("User/ChangePassword/{id}")]
        public IActionResult ChangePassword(int id)
        {
            return View(new ChangePassModel(){Id = id, Password = "12345678"});
        }
        
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassModel model)
        {
            if (ModelState.IsValid)
            {
                var user =await _userServices.FindAsync(model.Id);

                if (user is not null)
                {
                    if (User.GetRole() == (int)TypeAccountEnum.Member && user.PassWord != model.Password.GetHash())
                    {
                        ModelState.AddModelError("Password","رمز عبور فعلی درست نمی باشد.");
                        return View(model);
                    }

                    user.PassWord = model.NewPass.GetHash();
                    var rows=await _applicationContext.SaveChangesAsync();
                    if (rows>0)
                    {
                        model.State = 1;
                        ModelState.AddModelError("State", "رمز عبور با موفقیت تغییر کرد.");
                        user.SerialNo = Guid.NewGuid().ToString().Substring(0, 10);
                        return View(model);
                    }
                }
                }
            model.State = 2;
            ModelState.AddModelError("State", "خطا رد تغییر رمز عبور");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var exist =await _userServices.RemoveUser(id);
            if (!exist)
            {
                return Json(new {state = false, message = "کاربر یافت نشد."});
            }
            var row=await _applicationContext.SaveChangesAsync();
            if (row>0)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Json(new {state = true});
            }

            return Json(new {state = false, message = "حذف انجام نشد."});
        }
    }
}