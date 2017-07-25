using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Vege.Utils
{
    public class SHA1Encrypter
    {
        public static string encrypt(string origin)
        {
            byte[] originBytes = Encoding.UTF8.GetBytes(origin);
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(originBytes);
            return Encoding.UTF8.GetString(hash);
        }
    }
}
