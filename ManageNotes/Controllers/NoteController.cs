using System;
using System.Linq;
using System.Threading.Tasks;
using EPPlus.Core.Extensions.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IO;
using System.Threading;
using DNTPersianUtils.Core;
using MailKit.Net.Smtp;
using ManageNotes.ActionResult;
using ManageNotes.Data;
using ManageNotes.Services;
using ManageNotes.Utils;
using ManageNotes.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using MimeKit;

namespace ManageNotes.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private ApplicationContext _applicationContext;
        private NoteServices _noteServices;
        private UserServices _userServices;
        //private IMemoryCache _memoryCache;

        public NoteController(ApplicationContext applicationContext, 
            NoteServices noteServices, UserServices userServices)
        {
            _applicationContext = applicationContext;
            _noteServices = noteServices;
            _userServices = userServices;
            //_memoryCache = memoryCache;
        }

        public IActionResult NoteIndex()
        {
            
            return View();
        }

        public async Task<IActionResult> SendEmail()
        {
            var id = User.GetId();
            var user =await _userServices.FindAsync(id);

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Web Application","amir.hnaeemian@gmail.com"));
            email.To.Add(new MailboxAddress("Amir.H","Amir.hnaeemian2000@gmail.com"));
            email.Subject = "a Test Email";
            
            var builder = new BodyBuilder();
            builder.HtmlBody = "<div style=\"border: black 2px solid;text-align: center;align-items: center;justify-items: center;background-color: #9fcdff\">"+
                $"<p>نام کاربری : {user.UserName}</p>"+
                $"<p>نقش : {user.Role}</p>"+
                $"<p>تاریخ ایجاد اکانت : {user.CreatedDate}</p>"+
                "<a style=\"color: red\" href=\"https://localhost:5001/Home/Index\">انتقال به سایت</a>"+
                "</div>";

            var stream = new MemoryStream(user.Image);
            await builder.Attachments.AddAsync("my file.jpeg", stream);
            /*var wwrootpath = _environment.WebRootPath;
            builder.Attachments.Add(Path.Combine(wwrootpath,"image","presentntion.pptx"));*/

            email.Body = builder.ToMessageBody();

            using (var client=new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587,false);
                await client.AuthenticateAsync("amir.hnaeemian@gmail.com", "Amir13792000pm");
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
            
            return RedirectToAction("NoteIndex", "Note");
        }
        
        [HttpGet]
        public IActionResult New (int userId=-1)
        {
            if (userId != -1)
            {
                return View(new NewModel() {UserId = userId});
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> New(NewModel model)
        {
            if (ModelState.IsValid)
            {
                int userId= model.UserId;
                if (User.GetRole() == (int)TypeAccountEnum.Member)
                {
                    userId = User.GetId();
                }

                if (await _noteServices.IsExistNoteAsync(userId, model.Title))
                {
                    return Json(new {status=false, message="یادداشتی با این عنوان وجود دارد. عنوان دیگری انتخاب کنید."});
                }
                var note = new Note()
                {
                    MainNote = model.MineNote,
                    Title = model.Title,
                    User =await _userServices.FindAsync(userId)
                };
                await _noteServices.AddNoteAsync(note); 
                await _applicationContext.SaveChangesAsync();
                
                return Json(new {status=true, message="یادداشت ذخیره شد."});
            }
            
            
            return Json(new {status=false, message="یادذاشت ذخیره نشد"});
        }

        [HttpGet]
        [Route("Note/ShowNotes/{id}")]
        public IActionResult ShowNotes(int id=-1)
        {
            if (id != -1)
            {
                Response.Cookies.Append("userId", id.ToString(), new CookieOptions()
                {
                    Expires = DateTimeOffset.Now.AddMinutes(15)
                });
            }
            return View();
        }

        //[HttpPost]
        public async Task<IActionResult> GetNotes(int start, int length)
        {
            var filter = Request.Query["search[value]"].ToString();
            var userId = User.GetId();
            if (User.GetRole() != (int)TypeAccountEnum.Member)
            {
                userId = int.Parse(Request.Cookies["userId"]);
            }

            /*var notes = _memoryCache.Get<List<Note>>("listnote");
            if (notes ==null)
            {
                notes=await _noteServices.GetNotesWithUserIdsAsync(userId);
                _memoryCache.Set("listnote", notes, DateTimeOffset.Now.AddMinutes(1));
            }*/
            var notes=await _noteServices.GetNotesByUserIdsAsync(userId);

            var list = notes.OrderByDescending(x => x.CreatedDate).Select(x => new NewModel2()
            {
                NoteId = x.Id,
                Title = x.Title,
                Number =notes.Count - notes.IndexOf(x)
            }).ToList();

            var total = list.Count;
            var filteredcount = total;
            
            if (!String.IsNullOrEmpty(filter))
            {
                list = list.Where(x => x.Title.Contains(filter) || x.Number.ToString()==filter)
                    .ToList();
                filteredcount = list.Count;
            }

            list = list.Skip(start).Take(length).ToList();
            
            return Json(new {data = list, recordsTotal=total, recordsFiltered=filteredcount});
        }
        
        
        public class NewModel2
        {
            public int Number { get; set; }
            public string Title { get; set; }
            public int NoteId { get; set; }
        }

        public async Task<IActionResult> ExportNotes()
        {
            var id = User.GetId();
            if (User.GetRole() != (int)TypeAccountEnum.Member)
            {
                id = int.Parse(Request.Cookies["userId"]);
            }

            var list = await _noteServices.GetAllNotesAsync(id);
            
            if (list.Count==0)
            {
                return new EmptyResult();
            }

            var listExcel = list.Select(x => new NewModel3()
            {
                Title = x.Title,
                MainNote = x.MainNote,
                CreatedDate = x.CreatedDate.ToShortPersianDateTimeString()
            }).ToList();
            var writerNotes =await _userServices.FindAsync(id);
            var file =await ExcelWriter.GetExcelBytesAsync(listExcel);
            return new ExcelResult(file, writerNotes.UserName);
        }

        public static String GetDatetime(DateTime date)
        {
            var persian = new PersianCalendar();
            var year = persian.GetYear(date);
            var month = persian.GetMonth(date);
            var day = persian.GetDayOfMonth(date);
            return String.Format("{0}/{1}/{2}", year, month, day);
        }

        private class NewModel3
        {
            [ExcelTableColumn("عنوان")]
            public string Title { get; set; }
            
            [ExcelTableColumn("متن یادداشت")] 
            public String MainNote { get; set; }
            
            [ExcelTableColumn("تاریخ")] 
            public String CreatedDate { get; set; }
        }

        [HttpPost]
        [Route("Note/ShowNote")]
        public async Task<IActionResult> ShowNote(int id)
        {
            var note =await _noteServices.FindAsync(id);
            return PartialView("_ShowNote", note);
        }
        
        [HttpGet]
        public async Task<IActionResult> Modify(int id)
        {
            var note =await _noteServices.FindAsync(id);
            return View(new ModifywModel()
            {
                Title = note.Title,
                MineNote = note.MainNote,
                Id = id
            });
        }

        [HttpPost]
        public async Task<IActionResult> Modify(ModifywModel model)
        {
            if (ModelState.IsValid)
            {
                var noteId = model.Id;
                var postNote =await _noteServices.FindAsync(noteId);

                if (postNote.Title != model.Title && await _noteServices
                    .IsExistNoteAsync(model.Id, model.Title))
                    return Json(new {status = false, message = "یادداشتی با این عنوان وجود دارد. عنوان دیگری انتخاب کنید."});

                    if (postNote.Title==model.Title && postNote.MainNote==model.MineNote)
                    return Json(new {status = true, message = "یادداشت با موفقیت ویرایش شد."});
                
                postNote.Title = model.Title;
                postNote.MainNote = model.MineNote;
                var rows= await _applicationContext.SaveChangesAsync();
                if (rows>0)
                    return Json(new {status = true, message = "یادداشت با موفقیت ویرایش شد."});
            }
            
            return Json(new {status = false, message = "خطا در ویرایش یادداشت"});
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var note =await _noteServices.FindAsync(id);
            if (note is null)
            {
                return Json(new {state = false, message = "این یادداشت موجود نیست."});
            }

            _applicationContext.Notes.Remove(note);
            var row =await _applicationContext.SaveChangesAsync();
            return Json(new {state = row > 0});
        }

    }
}