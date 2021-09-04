
using System;
using System.Reflection;
using ManageNotes.Data;

namespace ManageNotes.Attributes
{
    public class TypeAccountValueAttribute:Attribute
    {
        public string Value;

        public TypeAccountValueAttribute(string value)
        {
            this.Value = value;
        }
    }

    public static class ExtensionMethods
    {
        public static string GetValue(this TypeAccountEnum model)
        {
            var type = model.GetType();
            var member = type.GetMember(model.ToString())[0];
            foreach (var VARIABLE in member.GetCustomAttributes(true))
            {
                var c = VARIABLE as TypeAccountValueAttribute;
                if (c is not null)
                {
                    return c.Value;
                }
            }

            return member.Name;
        }
    }
}