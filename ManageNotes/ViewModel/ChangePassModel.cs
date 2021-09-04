using System;
using System.ComponentModel.DataAnnotations;

namespace ManageNotes.ViewModel
{
    public class ChangePassModel
    {
        [Required(ErrorMessage = "وارد کردن رمز عبور اجباری است.")]
        [MinLength(8,ErrorMessage = "رمز عبور حداقل 8 نویسه است.")]
        public String Password { get; set; }
        
        [Required(ErrorMessage = "وارد کردن رمز عبور اجباری است.")]
        [MinLength(8,ErrorMessage = "رمز عبور باید حداقل 8 نویسه باشد.")]
        [Compare("RepeatNewPass",ErrorMessage = "رمز عبور و تکرار آن یکسان نیستند.")]
        public String NewPass { get; set; }
        
        [Required(ErrorMessage = "وارد کردن رمز عبور اجباری است.")]
        [MinLength(8,ErrorMessage = "رمز عبور باید حداقل 8 نویسه باشد.")]
        public String RepeatNewPass { get; set; }

        public int State { get; set; } = 0;

        public int Id { get; set; }
    }
}