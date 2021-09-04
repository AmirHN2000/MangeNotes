using System;
using ManageNotes.Data;

namespace ManageNotes.ViewModel
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public String UserName { get; set; }

        public TypeAccountEnum Role { get; set; }

        public int Number { get; set; }

        public String SerialNo { get; set; }
    }
}