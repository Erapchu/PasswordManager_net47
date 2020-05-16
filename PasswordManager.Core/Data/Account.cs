using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Core.Data
{
    [DebuggerDisplay("User password = {CorrectPassword}, Count of passwords = {Data.Count}")]
    [Serializable]
    public class Account : INotifyPropertyChanged
    {
        public CredentialsCollection Credentials { get; set; }

        private string _correctPassword;
        public string CorrectPassword
        {
            get
            {
                return _correctPassword;
            }
            set
            {
                _correctPassword = value;
                OnPropertyChanged();
            }
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
