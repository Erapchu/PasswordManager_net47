using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager
{
    class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<AccountData> AccountDatas { get; private set; }
        public MainViewModel()
        {
            AccountDatas = new ObservableCollection<AccountData>(AccountData.ReadFile());
        }

        private AccountData _selectedAccount;
        public AccountData SelectedAccount
        {
            get
            {
                return _selectedAccount;
            }
            set
            {
                _selectedAccount = value;
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
