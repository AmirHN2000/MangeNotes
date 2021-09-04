using System;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using ManageNotes.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using ManageNotes.Utils;

namespace ManageNotes.Attributes
{
    public class LogAttribute:Attribute,IAsyncActionFilter
    {
        private ApplicationContext _applicationContext;
        
        public LogAttribute(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();
            
            var log = new Log();
            log.ControllerName = context?.HttpContext?.Request?.RouteValues["controller"]?.ToString();
            log.ActionName=context?.HttpContext?.Request?.RouteValues["action"]?.ToString();
            log.IpAddress = context?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                //LocalIpAddress?.ToString();  ip of my server
            log.Browser = context?.HttpContext.Request.Headers["User-Agent"].ToString();
            log.PersianDate = DateTime.Now.ToShortPersianDateString();
            log.User =context.HttpContext.User.Identity.IsAuthenticated 
                ?await _applicationContext.Users.FindAsync(context.HttpContext.User.GetId())
                :null;
            await _applicationContext.Logs.AddAsync(log);
            await _applicationContext.SaveChangesAsync();
        }
    }
}