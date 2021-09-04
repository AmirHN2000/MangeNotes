using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ManageNotes.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using ManageNotes.Attributes;

namespace ManageNotes.ViewModel
{
    public class LogUpModel
    {
        [Required(ErrorMessage = "پر کردن فیلد نام کاربری اجباری است.")]
        [MinLength(3,ErrorMessage = "نام کاربری باید طولانی تر از سه حرف باشد.")]
        [MaxLength(200)]
        public String UserName { get; set; }

        [Required(ErrorMessage = "وارد کردن رمز عبور اجباری است.")]
        [MinLength(8,ErrorMessage = "رمز عبور باید طولانی تر از 8 نویسه باشد.")]
        [Compare("RepeatPassWord",ErrorMessage = "رمز عبور و تکرار آن یکسان نیستند.")]
        public String PassWord { get; set; }

        [Required(ErrorMessage = "وارد کردن تکرار رمز عبور اجباری است.")]
        [MinLength(8,ErrorMessage = "رمز عبور باید طولانی تر از 8 نویسه باشد.")]
        public String RepeatPassWord { get; set; }

        public IFormFile File { get; set; }
        public int State { get; set; } = 0;

        public TypeAccountEnum Role { get; set; } = TypeAccountEnum.Member;

        public List<SelectListItem> GetItem()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem(TypeAccountEnum.Member.GetValue(),((int)TypeAccountEnum.Member).ToString()));
            list.Add(new SelectListItem(TypeAccountEnum.Admin.GetValue(),((int)TypeAccountEnum.Admin).ToString()));
            return list;
        }
    }
}