using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using ManageNotes.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using MimeKit.Text;
using ManageNotes.Attributes;

namespace ManageNotes.Controllers
{
    //[TypeFilter(typeof(LogAttribute))]
    public class HomeController : Controller
    {
        //[TypeFilter(typeof(LogAttribute))]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> SendEmail()
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Web Application", "amir.hnaeemian@gmail.com"));
            email.To.Add(new MailboxAddress("Amir.H", "Amir.hnaeemian2000@gmail.com"));
            email.Subject = "a Test Email";
            email.Body = new TextPart(TextFormat.Html)
            {
                Text =
                    "<div><p style=\"color: red\">Use From Web Application To Create And Modify Your Notes.</p><br>" +
                    "<a href=\"https://localhost:5001/home/index\">Open Our Site</a></div>"
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("amir.hnaeemian@gmail.com", "Amir13792000pm");
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }

            return RedirectToAction("Index", "Home");
        }

        public List<SelectListItem> GetItems()
        {
            var list = new List<SelectListItem>();
            foreach (TypeAccountEnum variable in Enum.GetValues(typeof(TypeAccountEnum)))
            {
                list.Add(new SelectListItem(variable.GetValue(), ((int)variable).ToString()));
            }
            return list;
        }
        
    }
}