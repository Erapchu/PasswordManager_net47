using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class EncryptDecrypt
    {
        public static byte[] Decrypt(string FILE_NAME, int KEY)
        {
            try
            {
                byte[] buf = File.ReadAllBytes(FILE_NAME);

                for (int i = 0; i < buf.Length; i++)
                {
                    buf[i] = (byte)(buf[i] ^ KEY);
                }
                return buf;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (Exception)
            {
                AttentionW win = new AttentionW();
                win.InformationText.Text = "Error";
                win.ShowDialog();
            }
            return null;
        }

        public static byte[] Encrypt(MemoryStream stream, int KEY)
        {
            byte[] buf = stream.ToArray();

            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)(buf[i] ^ KEY);
            }
            return buf;
        }

        public static byte[] EncryptPass(string pass, int KEY)
        {
            MemoryStream stream = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(stream))
            {
                bw.Write(pass);
            }
            return Encrypt(stream, KEY);
        }
    }
}
