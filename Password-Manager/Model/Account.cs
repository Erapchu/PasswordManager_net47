using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager.Model
{
    class Account : INotifyPropertyChanged
    {
        public ObservableCollection<AccountData> Data { get; set; }

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
