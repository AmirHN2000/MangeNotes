using System;
using System.ComponentModel.DataAnnotations;

namespace ManageNotes.ViewModel
{
    public class ModifywModel
    {
        [Required(ErrorMessage = "پر کردن فیلد عنوان اجباری است.")]
        public String Title { get; set; }

        [Required(ErrorMessage = "پر کردن فیلد متن یادداشت اجباری است.")]
        public String MineNote { get; set; }
        public int Id { get; set; }
    }
}