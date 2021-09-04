using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DNTPersianUtils.Core;

namespace ManageNotes.Data
{
    public class Payment:BaseEntity
    {
        public Payment()
        {
            Syst_Code = Guid.NewGuid().ToString().Substring(0, 30).Replace("-", "");
        }
        
        [StringLength(30)]
        public string Syst_Code { get; set; }

        [MaxLength(50)]
        public string Ref_code { get; set; }

        [MaxLength(30)]
        public string PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }

        public long Amount { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }
        
        
    }
}