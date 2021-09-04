using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ManageNotes.Data
{
    public class User:BaseEntity
    {
        public User()
        {
            SerialNo =Guid.NewGuid().ToString().Substring(0,10);
        }

        [StringLength(100)]
        public String UserName { get; set; }
        [StringLength(50)]
        public String PassWord { get; set; }

        public TypeAccountEnum Role { get; set; }

        [StringLength(10)]
        public String SerialNo { get; set; }
        public byte[] Image { get; set; }

        public long Credit { get; set; }
        
        public List<Note> List { get; set; }
    }
}