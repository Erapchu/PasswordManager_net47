using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Core.Helpers
{
    public static class Encryption
    {
        public static string Process(string s, int KEY)
        {
            byte[] buf = Encoding.Unicode.GetBytes(s);

            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)(buf[i] ^ KEY);
            }
            string result = Encoding.Unicode.GetString(buf);
            return result;
        }
    }
}
