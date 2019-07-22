using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager
{
    static class FileProcess
    {
        private static string PathToMainFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\testdata.dat";

        public static void WriteFile(AccountData[] datas)
        {
            try
            {
                using (BinaryWriter bw = new BinaryWriter(File.Open(PathToMainFile, FileMode.Create)))
                {
                    foreach (AccountData var in datas)
                    {
                        bw.Write(var.Name);
                        bw.Write(var.Login);
                        bw.Write(var.Password);
                        bw.Write(var.Other);
                    }
                }
            }
            catch
            {
                //ошибки обработать
            }

        }

        public static AccountData[] ReadFile()
        {
            try
            {
                List<AccountData> accountDatas = new List<AccountData>();
                MemoryStream stream = new MemoryStream(File.ReadAllBytes(PathToMainFile));
                using (BinaryReader br = new BinaryReader(stream))
                {
                    AccountData data;
                    while (br.PeekChar() != -1)
                    {
                        data = new AccountData();
                        data.Name = br.ReadString();
                        data.Login = br.ReadString();
                        data.Password = br.ReadString();
                        data.Other = br.ReadString();
                        accountDatas.Add(data);
                    }
                }
                return accountDatas.ToArray();
            }
            catch(FileNotFoundException)
            {
                File.Create(PathToMainFile);
                return null;
            }
            catch
            {
                //обработать тут остальные ошибушки
                return null;
            }
        }

    }
}
