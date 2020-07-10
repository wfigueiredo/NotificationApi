using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationApi.Util
{
    public class HashUtil
    {
        public static string SHA1(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);

            using var hasher = System.Security.Cryptography.SHA1.Create();
            var hashedBytes = hasher.ComputeHash(bytes);

            var sb = new StringBuilder();
            for (var i = 0; i < hashedBytes.Length; i++)
            {
                sb.Append(hashedBytes[i].ToString());
            }

            return sb.ToString();
        }
    }
}
