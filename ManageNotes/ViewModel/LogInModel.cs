using System;
using System.ComponentModel.DataAnnotations;

namespace ManageNotes.ViewModel
{
    public class LogInModel
    {
        [Required(ErrorMessage = "وارد کردن نام کاربری اجباری است.")]
        public String UserName { get; set; }

        [Required(ErrorMessage = "وارد کردن رمز عبور اجباری است.")]
        [MinLength(8,ErrorMessage = "رمز عبور باید بیشتر از 8 نویسه باشد.")]
        public String PassWord { get; set; }

        public bool State { get; set; }
    }
}