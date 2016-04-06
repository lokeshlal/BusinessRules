using System;

namespace BusinessRules.Common
{
    public static class EnumExtensions
    {
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo[0].MemberType == System.Reflection.MemberTypes.Method)
            {
                var attributes = memInfo[1].GetCustomAttributes(typeof(T), false);
                return (attributes.Length > 0) ? (T)attributes[0] : null;
            }
            else {
                var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
                return (attributes.Length > 0) ? (T)attributes[0] : null;
            }
        }
    }
}
