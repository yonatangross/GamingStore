using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GamingStore.Extensions
{
    public static class EnumExtension
    {
        public static string GetDisplayName(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo.Length <= 0)
            {
                return en.ToString();
            }

            object[] attributes = memInfo[0].GetCustomAttributes(typeof(DisplayAttribute), true);
            return attributes.Length > 0 ? ((DisplayAttribute)attributes[0]).Name : en.ToString();
        }
    }
}
