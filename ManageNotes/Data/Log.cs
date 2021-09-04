using System.ComponentModel.DataAnnotations.Schema;

namespace ManageNotes.Data
{
    public class Log:BaseEntity
    {
        public Log()
        {
            User =new User() ;
        }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int? UserId { get; set; }
        public string IpAddress { get; set; }
        public string Browser { get; set; }
        public string PersianDate { get; set; }
    }
}