using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageNotes.Data
{
    public class Note:BaseEntity
    {
        [StringLength(100)]
        public String Title { get; set; }
        
        public String MainNote { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }
    }
}