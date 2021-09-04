using System.Threading.Tasks;
using ManageNotes.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ManageNotes.Components
{
    public class AdsViewComponent:ViewComponent
    {
        private IConfiguration _configuration;

        public AdsViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var link = _configuration["ads:default"];
            var l = new AdsModel()
            {
                Link = link
            };
            return View("_default",l);
        }
    }
}