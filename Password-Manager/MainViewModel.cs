using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Password_Manager
{
    class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<AccountData> DataOfAccount { get; private set; }
        public MainViewModel()
        {
            //Получение данных из файла
            AccountData[] account_data = File_process.ReadFile();
            if (account_data != null) DataOfAccount = new ObservableCollection<AccountData>(account_data);
            else DataOfAccount = new ObservableCollection<AccountData>();

            //Инициализация
            //AddCommand = new DelegateCommand();
            //RemoveCommand = new DelegateCommand();
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

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }

        #region MVVM Pattern
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
