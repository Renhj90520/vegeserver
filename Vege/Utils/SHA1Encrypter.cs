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

            //byte[] originBytes = Encoding.UTF8.GetBytes(origin);
            ASCIIEncoding enc = new ASCIIEncoding();
            var originBytes = enc.GetBytes(origin);
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(originBytes);

            var tempStr = BitConverter.ToString(hash);
            tempStr = tempStr.Replace("-", "").ToLower();
            return tempStr;
        }
    }
}
