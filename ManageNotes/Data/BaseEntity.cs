using System;
using System.ComponentModel.DataAnnotations;

namespace ManageNotes.Data
{
    public class BaseEntity
    {
        protected BaseEntity()
        {
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
