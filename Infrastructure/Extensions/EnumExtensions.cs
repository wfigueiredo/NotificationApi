using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NotificationApi.Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumType)
        {
            return enumType.GetType()
                        .GetMember(enumType.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        .Name;
        }
    }
}
