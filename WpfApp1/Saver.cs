using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;

namespace WpfApp1
{
    class Saver
    {

        /// <summary>
        /// Write encrypted file to My Documents
        /// </summary>
        /// <param name="CorrectPassword">Header of file - correct password</param>
        /// <param name="AccountsList">Data what from ListBox</param>
        /// <param name="MyDocuments">Path to My Documents</param>
        public static void Save(string CorrectPassword, ListBox AccountsList, string MyDocuments)
        {
            MemoryStream stream = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(stream))
            {
                bw.Write(CorrectPassword);
                foreach (DataAccount var in AccountsList.Items)
                {
                    bw.Write(var.Name);
                    bw.Write(var.Login);
                    bw.Write(var.Password);
                    bw.Write(var.OtherInf);
                }
            }
            byte[] TextToFile = EncryptDecrypt.Encrypt(stream, 4);
            using (BinaryWriter bw = new BinaryWriter(File.Open(MyDocuments + @"\passdata.dat", FileMode.Create)))
            {
                bw.Write(TextToFile);
            }
        }
    }
}
