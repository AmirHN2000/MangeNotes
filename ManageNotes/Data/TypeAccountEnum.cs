using ManageNotes.Attributes;

namespace ManageNotes.Data
{
    public enum TypeAccountEnum
    {
        [TypeAccountValue("مالک")]
        Owner,
        
        [TypeAccountValue("ادمین")]
        Admin,
        
        [TypeAccountValue("کاربر")]
        Member
    }

    public static class ExtensionClass
    {
        public static string GetNumberWithString(this TypeAccountEnum model)
        {
            return ((int) model).ToString();
        }
    } 
}