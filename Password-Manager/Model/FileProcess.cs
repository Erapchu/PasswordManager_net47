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
        private readonly int DefaultRandomSize;
        public FileProcess()
        {
            PathToMainFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\testdata.dat";
            DefaultRandomSize = 20;
        }

        public bool WriteFile(AccountData[] datas, string correctPass, out string status)
        {
            try
            {
                Encryption.KEY = new Random().Next(1, DefaultRandomSize);

                using (BinaryWriter bw = new BinaryWriter(File.Open(PathToMainFile, FileMode.Create)))
                {
                    //Header Ключ для шифрования и зашифрованный пароль
                    bw.Write(Encryption.KEY);
                    bw.Write(Encryption.Process(correctPass));

                    //Зашифрованные данные
                    foreach (AccountData data in datas)
                    {
                        bw.Write(Encryption.Process(data.Name));
                        bw.Write(Encryption.Process(data.Login));
                        bw.Write(Encryption.Process(data.Password));
                        bw.Write(Encryption.Process(data.Other));
                    }
                }
                status = "Файл сохранён";
                return true;
            }
            catch
            {
                status = "Файл не был сохранен успешно";
                return false;
            }
        }

        private long offsetToRead;

        public AccountData[] ReadFile(out string status)
        {
            try
            {
                List<AccountData> accountDatas = new List<AccountData>();
                using (BinaryReader br = new BinaryReader(File.Open(PathToMainFile, FileMode.Open)))
                {
                    br.BaseStream.Position = offsetToRead;
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
            catch
            {
                status = "Файл повреждён";
                return null;
            }
        }

        public string ReadPassword(out string status)
        {
            try
            {
                string pass;
                using (BinaryReader br = new BinaryReader(File.Open(PathToMainFile, FileMode.Open)))
                {
                    //Получение ключа для дешифрования
                    Encryption.KEY = br.ReadInt32();
                    //Чтение пароля
                    pass = Encryption.Process(br.ReadString());
                    //Сохранение смещения позиции
                    offsetToRead = br.BaseStream.Position;
                }
                status = "";
                return pass;
            }
            catch (FileNotFoundException)
            {
                File.Create(PathToMainFile);
                status = "Новый файл создан, потребуется пароль при сохранении";
                return null;
            }
            catch
            {
                status = "Поврежден заголовок файла";
                return null;
            }
        }
    }
}
