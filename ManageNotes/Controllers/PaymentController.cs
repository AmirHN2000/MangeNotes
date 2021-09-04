using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ManageNotes.Data;
using ManageNotes.Services;
using Microsoft.AspNetCore.Mvc;
using Parbad;
using RestSharp;
using ManageNotes.Utils;

namespace ManageNotes.Controllers
{
    public class PaymentController : Controller
    {
        private ApplicationContext _applicationContext;
        private IOnlinePayment _onlinePayment;
        private PaymentServices _paymentServices;

        public PaymentController(IOnlinePayment onlinePayment, ApplicationContext applicationContext, PaymentServices paymentServices)
        {
            _onlinePayment = onlinePayment;
            _applicationContext = applicationContext;
            _paymentServices = paymentServices;
        }

        [HttpPost]
        public async Task<IActionResult> Pay(int amount)
        {
            var payment = new Payment()
            {
                Amount = amount,
                UserId = User.GetId()
            };
            var username = User.Claims.First(x => x.Type == ClaimTypes.Name).Value;
            
            var client = new RestClient("https://api.idpay.ir/v1.1/payment");
            var request = new RestRequest(Method.POST);
            request.AddParameter("X-API-KEY", "75f79830-1e02-42c6-a13d-a1da83d164c5");
            request.AddParameter("X-SANDBOX", 1);
            request.AddParameter("order_id", payment.Syst_Code);
            request.AddParameter("amount", payment.Amount);
            request.AddParameter("name", username);
            request.AddParameter("callback", "https://localhost:5001/payment/Verify"); //****** change for Use in IIS ******//

            var response =await client.ExecuteAsync(request);
            if (response.IsSuccessful && response.StatusCode==HttpStatusCode.Created)
            {
                var link=response.ResponseUri.AbsoluteUri;
                var id = response.Headers[12].Value.ToString();
                
                return Redirect(Path.Combine(link, id));
            }

            return Content(response.ErrorException.Message);
            
            //var id = response?.Headers[12].Value?.ToString();
            //return Redirect(Path.Combine(link, payment.Syst_Code));
        }

        [HttpGet]
        public IActionResult Verify()
        {
            return Content("in action");
        }
    }
}