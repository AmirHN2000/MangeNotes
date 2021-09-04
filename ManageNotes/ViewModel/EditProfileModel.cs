using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ManageNotes.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using ManageNotes.Attributes;

namespace ManageNotes.ViewModel
{
    public class EditProfileModel
    {
        [Required(ErrorMessage = "پر کردن فیلد نام کاربری اجباری است.")]
        [MinLength(3,ErrorMessage = "نام کاربری باید طولانی تر از سه حرف باشد.")]
        [MaxLength(200)]
        public String UserName { get; set; }
        
        public IFormFile File { get; set; }
        
        public int Id { get; set; }

        public TypeAccountEnum Role { get; set; }

        public int Check { get; set; }

        public List<SelectListItem> GetItem()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem(TypeAccountEnum.Member.GetValue(),((int)TypeAccountEnum.Member).ToString()));
            list.Add(new SelectListItem(TypeAccountEnum.Admin.GetValue(),((int)TypeAccountEnum.Admin).ToString()));
            return list;
        }
    }
}