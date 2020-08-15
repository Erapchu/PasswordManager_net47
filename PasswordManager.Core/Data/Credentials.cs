using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Core.Data
{
    [DebuggerDisplay("Name = {Name}, Login = {Login}, Password = {Password}")]
    [Serializable]
    public class Credentials : INotifyPropertyChanged, ICloneable
    {

        public Credentials(string name, string login, string password, string other) : this()
        {
            Name = name;
            Login = login;
            Password = password;
            Other = other;
        }

        public Credentials()
        {
            ID = Guid.NewGuid();
        }

        public Guid ID { get; private set; }

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

        private DateTime _lastTimeUsage;
        public DateTime LastTimeUsage
        {
            get => _lastTimeUsage;
            set
            {
                _lastTimeUsage = value;
                OnPropertyChanged();
            }
        }

        private DateTime _creationTime;
        public DateTime CreationTime
        {
            get => _creationTime;
            set
            {
                _creationTime = value;
                OnPropertyChanged();
            }
        }

        public object Clone()
        {
            Credentials clone = new Credentials();
            clone.ID = this.ID;
            clone.Login = this.Login;
            clone.Name = this.Name;
            clone.Other = this.Other;
            clone.Password = this.Password;
            clone.LastTimeUsage = this.LastTimeUsage;
            clone.CreationTime = this.CreationTime;
            return clone;
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
