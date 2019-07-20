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
            AddCommand = new DelegateCommand(AddAccount);
            RemoveCommand = new DelegateCommand(RemoveAccount, canRemoveAccount);
            IsEditMode = new EditMode(false);
        }

        public EditMode IsEditMode { get; set; }

        public ICommand AddCommand { get; private set; }
        private void AddAccount(object obj)
        {
            SelectedAccount = null;
            IsEditMode.Switch(true);
            //делать новый биндинг на ... текстбоксы? для редактирования.
            //+ создать EditableAccount и его забиндить на тестбоксы, работать с ним
        }

        public ICommand RemoveCommand { get; private set; }
        private void RemoveAccount(object obj)
        {
            DataOfAccount.Remove((AccountData)obj);
        }

        private bool canRemoveAccount(object arg)
        {
            return (arg as AccountData) != null;
        }

        public ICommand ChangeCommand { get; private set; }
        private void ChangeAccount()
        {

        }

        public ICommand SaveCommand { get; private set; }
        private void SaveData()
        {

        }

        public ICommand AcceptEditCommand { get; private set; }
        private void AcceptEdit()
        {
            //DataOfAccount.Add(new AccountData { Login = })
        }

        public ICommand DeclineEditCommand { get; private set; }
        private void DeclineEdit()
        {

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
