using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager
{
    class AccountData : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _login;
        public string Login
        {
            get
            {
                return _login;
            }
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _other;
        public string Other
        {
            get
            {
                return _other;
            }
            set
            {
                _other = value;
                OnPropertyChanged();
            }
        }

        public static AccountData[] ReadFile()
        {
            string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            List<AccountData> accountDatas = new List<AccountData>();
            MemoryStream stream = new MemoryStream(File.ReadAllBytes(MyDocuments + @"\testdata.dat"));
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

        #region MVVM Pattern
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
