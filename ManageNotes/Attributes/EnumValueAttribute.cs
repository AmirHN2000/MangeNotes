using System;
using ManageNotes.Models;

namespace ManageNotes.Attributes
{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple = false)]
    public class EnumValueAttribute:Attribute
    {
        public string Value;

        public EnumValueAttribute(string value)
        {
            Value = value;
        }
    }

    public static class EnumExtension
    {
        public static string GetValue(this CitiesEnum model)
        {
            var type = model.GetType();
            var member = type.GetMember(model.ToString())[0];
            foreach (var VARIABLE in member.GetCustomAttributes(true))
            {
                var attr = VARIABLE as EnumValueAttribute;
                if (attr is not null)
                {
                    return attr.Value;
                }
            }

            return member.Name;
        }
    }
}