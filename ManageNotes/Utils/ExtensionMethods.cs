using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using ManageNotes.Data;
using ManageNotes.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace ManageNotes.Utils
{
    public static class ExtensionMethods
    {
        public async static Task<byte[]> GetArrayBytesAsync(this IFormFile file)
        {
            var d=new byte[file.Length];
            var m = new MemoryStream();
            await file.CopyToAsync(m);
            m.Seek(0, SeekOrigin.Begin);
            await m.ReadAsync(d);
            return d;
        }

        public static String GetHash(this String value)
        {
            var sha256 = new SHA256Managed();
            var bytes = Encoding.ASCII.GetBytes(value);
            var hashed = sha256.ComputeHash(bytes);
            var result = Encoding.ASCII.GetString(hashed);
            return result;
        }

        public static ClaimsPrincipal GetPrincipal(this UserViewModel model)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, model.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, model.Role.GetNumberWithString()));
            identity.AddClaim(new Claim("SerialNo",model.SerialNo));

            var principal = new ClaimsPrincipal(identity);
            return principal;
        }

        public static int GetId(this ClaimsPrincipal? principal)
        {
            if (principal is  null)
            {
                return -1;
            }
            var id = int.Parse(principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value.ToString());
            return id;
        }

        public static int GetRole(this ClaimsPrincipal principal)
        {
            var s = principal.Claims.First(x => x.Type == ClaimTypes.Role).Value.ToString();
            return int.Parse(s);
        }
        
        public static string GetSerialNo(this ClaimsPrincipal principal)
        {
            var s = principal.Claims.First(x => x.Type == "SerialNo").Value.ToString();
            return s;
        }
        
        public static (String controllerName, string actionName) SplitUrl(this string url)
        {
            var splited = url.Split('/');
            return (splited[1], splited[2]);
        }

        public static void DoSuccess(this Payment payment, string ref_code)
        {
            payment.Status = PaymentStatus.Success;
            payment.PaymentDate = DateTime.Now.ToShortPersianDateTimeString();
            payment.Ref_code = ref_code;
        }
    }
}