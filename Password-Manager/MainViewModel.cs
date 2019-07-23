using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Password_Manager
{
    class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<AccountData> DataOfAccount { get; private set; }
        public ICollectionView FilteringCollection { get; private set; }
        public EditMode IsEditMode { get; set; }
        private bool IsSaved { get; set; }

        private AccountData _ChangableAcount;
        public AccountData ChangableAccount
        {
            get
            {
                return _ChangableAcount;
            }
            set
            {
                _ChangableAcount = value;
                OnPropertyChanged();
            }
        }

        private AccountData _SelectedAccount;
        public AccountData SelectedAccount
        {
            get
            {
                return _SelectedAccount;
            }
            set
            {
                _SelectedAccount = value;
                OnPropertyChanged();
            }
        }

        private string _FilterText;
        public string FilterText
        {
            get
            {
                return _FilterText;
            }
            set
            {
                _FilterText = value;
                FilteringCollection.Refresh();
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            //Получение данных из файла
            AccountData[] account_data = FileProcess.ReadFile();

            //Инициализация
            AddCommand = new DelegateCommand(AddAccount);
            RemoveCommand = new DelegateCommand(RemoveAccount, CanRemoveAccount);
            ChangeCommand = new DelegateCommand(ChangeAccount, CanChangeAccount);
            SaveCommand = new DelegateCommand(SaveAll, CanSaveAll);
            AcceptEditCommand = new DelegateCommand(AcceptEdits);
            DeclineEditCommand = new DelegateCommand(DeclineEdits);
            ClearCommand = new DelegateCommand(ClearFilteredText, CanClearFilteredText);

            IsEditMode = new EditMode(false, false);
            if (account_data != null) DataOfAccount = new ObservableCollection<AccountData>(account_data);
            else DataOfAccount = new ObservableCollection<AccountData>();
            FilteringCollection = CollectionViewSource.GetDefaultView(DataOfAccount);
            FilteringCollection.Filter = FilterAccounts;
            IsSaved = true;
        }

        private bool FilterAccounts(object obj)
        {
            bool result = true;
            AccountData data = obj as AccountData;
            if (data != null && !string.IsNullOrWhiteSpace(FilterText) && !data.Name.Contains(FilterText)) return false;
            return result;
        }

        private bool CheckEmptyInput()
        {
            if (string.IsNullOrEmpty(SelectedAccount.Login) || string.IsNullOrEmpty(SelectedAccount.Name) || string.IsNullOrEmpty(SelectedAccount.Password)) return false;
            else return true;
        }

        #region Delegate commands
        private bool CanClearFilteredText(object arg)
        {
            return !string.IsNullOrEmpty((string)arg);
        }

        private bool CanSaveAll(object arg)
        {
            return !IsSaved;
        }

        private bool CanChangeAccount(object arg)
        {
            return (arg as AccountData) != null;
        }

        private bool CanRemoveAccount(object arg)
        {
            return (arg as AccountData) != null;
        }

        private void ClearFilteredText(object obj)
        {
            FilterText = string.Empty;
        }

        private void DeclineEdits(object obj)
        {
            if ((bool)obj)
            {
                DataOfAccount[DataOfAccount.IndexOf(SelectedAccount)] = ChangableAccount;
                SelectedAccount = ChangableAccount;
                IsEditMode.Switch(false, false);
            }
            else
            {
                SelectedAccount = null;
                IsEditMode.Switch(false);
            }
        }

        private void AcceptEdits(object obj)
        {
            if (CheckEmptyInput())
            {
                if ((bool)obj)
                {
                    IsEditMode.Switch(false, false);
                }
                else
                {
                    DataOfAccount.Add(new AccountData
                    {
                        Login = SelectedAccount.Login,
                        Name = SelectedAccount.Name,
                        Other = SelectedAccount.Other,
                        Password = SelectedAccount.Password
                    });
                    SelectedAccount = DataOfAccount.Last();
                    IsEditMode.Switch(false);
                }
                IsSaved = false;
            }
        }

        private void SaveAll(object obj)
        {
            FileProcess.WriteFile((obj as ObservableCollection<AccountData>).ToArray());
            IsSaved = true;
        }

        private void ChangeAccount(object obj)
        {
            ChangableAccount = new AccountData
            {
                Login = (obj as AccountData).Login,
                Name = (obj as AccountData).Name,
                Other = (obj as AccountData).Other,
                Password = (obj as AccountData).Password
            };
            IsEditMode.Switch(true, true);
        }

        private void RemoveAccount(object obj)
        {
            DataOfAccount.Remove((AccountData)obj);
            IsSaved = false;
        }

        private void AddAccount(object obj)
        {
            SelectedAccount = new AccountData();
            IsEditMode.Switch(true);
        }
        #endregion

        #region Commands of buttons
        public ICommand AddCommand { get; private set; }

        public ICommand RemoveCommand { get; private set; }

        public ICommand ChangeCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public ICommand AcceptEditCommand { get; private set; }

        public ICommand DeclineEditCommand { get; private set; }

        public ICommand ClearCommand { get; private set; }
        #endregion

        #region MVVM Pattern
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        
    }
}
