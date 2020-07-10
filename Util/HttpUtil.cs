using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NotificationApi.Util
{
    public class HttpUtil

    {
        public static bool IsSuccessStatusCode(HttpStatusCode httpStatusCode)
        {
            return ((int)httpStatusCode >= 200) && ((int)httpStatusCode <= 299);
        }

        public static bool IsValidEmail(string Email)
        {
            return new EmailAddressAttribute().IsValid(Email);
        }
    }
}
