using Password_Manager.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager
{
    class FileProcess
    {
        private readonly string PathToMainFile;
        private string CorrectPassword;
        public FileProcess()
        {
            PathToMainFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\testdata.dat";
        }
        public void WriteFile(AccountData[] datas, string correctPass, out string status)
        {
            try
            {
                Encryption.KEY = new Random().Next(1, 10);
                CorrectPassword = correctPass;

                using (BinaryWriter bw = new BinaryWriter(File.Open(PathToMainFile, FileMode.Create)))
                {
                    WriteHeader(bw);
                    WriteContent(datas, bw);
                }
                status = "Файл сохранён";
            }
            catch
            {
                status = "Файл не был сохранен успешно";
            }
        }

        private void WriteHeader(BinaryWriter bw)
        {
            //Ключ для шифрования
            bw.Write(Encryption.KEY);
            
            //Зашифрованный пароль
            bw.Write(Encryption.Encrypt(CorrectPassword));
        }

        private void WriteContent(AccountData[] datas, BinaryWriter bw)
        {
            foreach (AccountData data in datas)
            {
                bw.Write(Encryption.Encrypt(data.Name));
                bw.Write(Encryption.Encrypt(data.Login));
                bw.Write(Encryption.Encrypt(data.Password));
                bw.Write(Encryption.Encrypt(data.Other));
            }
        }

        public AccountData[] ReadFile(out string status)
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
