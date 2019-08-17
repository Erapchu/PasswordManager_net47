using GalaSoft.MvvmLight;
using Password_Manager.Model;
using Password_Manager.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Password_Manager
{
    public enum PassOperation
    {
        NewUser,
        DefaultUser,
        ChangePassword
    }
    class MainViewModel : INotifyPropertyChanged
    {
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

        public Account ThisAccount { get; set; }

        public MainViewModel()
        {
            //Получение данных из файла
            ThisAccount = FileProcess.Instance.ReadFile();
            if(ThisAccount == null)
            {
                ThisAccount = new Account();
            }

            InputPassView inputPassView;
            if (ThisAccount.CorrectPassword == null)
            {
                //Новый пользователь
                inputPassView = new InputPassView(string.Empty, PassOperation.NewUser);
                if (inputPassView.ShowDialog() == true)
                {
                    ThisAccount.CorrectPassword = inputPassView.inputPassViewModel.CorrectPassword;
                    ThisAccount.Data = new ObservableCollection<AccountData>();
                }
            }
            else
            {
                //Уже с паролем
                inputPassView = new InputPassView(ThisAccount.CorrectPassword, PassOperation.DefaultUser);
                inputPassView.ShowDialog();
            }

            //Инициализация
            AddCommand = new DelegateCommand(AddAccount);
            RemoveCommand = new DelegateCommand(RemoveAccount, CanRemoveAccount);
            ChangeCommand = new DelegateCommand(ChangeAccount, CanChangeAccount);
            SaveCommand = new DelegateCommand(SaveAll, CanSaveAll);
            AcceptEditCommand = new DelegateCommand(AcceptEdits);
            DeclineEditCommand = new DelegateCommand(DeclineEdits);
            ClearCommand = new DelegateCommand(ClearFilteredText, CanClearFilteredText);

            IsEditMode = new EditMode(false, false);
            FilteringCollection = CollectionViewSource.GetDefaultView(ThisAccount.Data);
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
                ThisAccount.Data[ThisAccount.Data.IndexOf(SelectedAccount)] = ChangableAccount;
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
                    ThisAccount.Data.Add(new AccountData
                    {
                        Login = SelectedAccount.Login,
                        Name = SelectedAccount.Name,
                        Other = SelectedAccount.Other == null ? string.Empty : SelectedAccount.Other,
                        Password = SelectedAccount.Password
                    });
                    SelectedAccount = ThisAccount.Data.Last();
                    IsEditMode.Switch(false);
                }
                IsSaved = false;
            }
            else MessageBox.Show("Please, fill this data: Name, Login, Password", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveAll(object obj)
        {
            if (!string.IsNullOrWhiteSpace(ThisAccount.CorrectPassword))
                FileProcess.Instance.WriteFile(obj as Account);
            else
            {
                if (new InputPassView().ShowDialog() == true)
                    FileProcess.Instance.WriteFile(obj as Account);
            }
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
            ThisAccount.Data.Remove((AccountData)obj);
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

        /* динамическое создание делегатов, анонимные методы
         * public ICommand AnyCommand
         * {
         *     get
         *     {
         *         return new DelegateCommand((obj) =>
         *         {
         *             //что-то выполняется
         *         });
         *     }
         * }
         */

    }
}
