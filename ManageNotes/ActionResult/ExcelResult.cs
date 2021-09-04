using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ManageNotes.ActionResult
{
    public class ExcelResult:IActionResult
    {
        private byte[] _bytes;
        private String _name;

        public ExcelResult(byte[] bytes, string name="MyNotes")
        {
            _bytes = bytes;
            _name = name;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.Headers.Add("Content-Disposition",
                $"attachment; filename=\"{_name}.xlsx\"");
            var file = new FileContentResult(_bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            await file.ExecuteResultAsync(context);
        }
    }
}