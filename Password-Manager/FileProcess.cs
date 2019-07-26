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
        private static readonly string PathToMainFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\testdata.dat";

        public static void WriteFile(AccountData[] datas, out string status)
        {
            try
            {
                using (BinaryWriter bw = new BinaryWriter(File.Open(PathToMainFile, FileMode.Create)))
                {
                    foreach (AccountData data in datas)
                    {
                        bw.Write(data.Name);
                        bw.Write(data.Login);
                        bw.Write(data.Password);
                        bw.Write(data.Other);
                    }
                }
                status = "Файл сохранён";
            }
            catch
            {
                status = "Файл не был сохранен успешно";
            }
        }

        public static AccountData[] ReadFile(out string status)
        {
            try
            {
                List<AccountData> accountDatas = new List<AccountData>();
                using (BinaryReader br = new BinaryReader(File.Open(PathToMainFile, FileMode.Open)))
                {
                    AccountData data;
                    while (br.PeekChar() != -1)
                    {
                        data = new AccountData
                        {
                            Name = br.ReadString(),
                            Login = br.ReadString(),
                            Password = br.ReadString(),
                            Other = br.ReadString()
                        };
                        accountDatas.Add(data);
                    }
                }
                status = "";
                return accountDatas.ToArray();
            }
            catch(FileNotFoundException)
            {
                File.Create(PathToMainFile);
                status = "Новый файл создан";
                return null;
            }
            catch
            {
                status = "Файл повреждён";
                return null;
            }
        }

    }
}
