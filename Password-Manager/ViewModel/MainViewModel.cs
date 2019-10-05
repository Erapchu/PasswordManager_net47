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
            //Get data from file
            ThisAccount = FileProcess.Instance.ReadFile();
            if (ThisAccount == null)
            {
                ThisAccount = new Account();
            }

            //Try to do multi accounts?

            InputPassView inputPassView;
            if (ThisAccount.CorrectPassword == null)
            {
                //Новый пользователь
                inputPassView = new InputPassView(string.Empty, PassOperation.NewUser);
                if (inputPassView.ShowDialog() == true)
                {
                    ThisAccount.Data = new ObservableCollection<AccountData>();
                    ThisAccount.CorrectPassword = inputPassView.inputPassViewModel.CorrectPassword;
                    FileProcess.Instance.WriteFile(ThisAccount);
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                //Already with password
                inputPassView = new InputPassView(ThisAccount.CorrectPassword, PassOperation.DefaultUser);
                if (inputPassView.ShowDialog() == false) Environment.Exit(0);
            }

            //Init
            AddCommand = new DelegateCommand(AddAccountData);
            RemoveCommand = new DelegateCommand(RemoveAccountData, CanRemoveAccountData);
            ChangeCommand = new DelegateCommand(ChangeAccountData, CanChangeAccountData);
            SaveCommand = new DelegateCommand(SaveAll, CanSaveAll);
            AcceptEditCommand = new DelegateCommand(AcceptEdits);
            DeclineEditCommand = new DelegateCommand(DeclineEdits);
            ClearCommand = new DelegateCommand(ClearFilteredText, CanClearFilteredText);

            IsEditMode = new EditMode(false, false);
            FilteringCollection = CollectionViewSource.GetDefaultView(ThisAccount.Data);
            FilteringCollection.Filter = FilterAccountDatas;
            IsSaved = true;

        }

        private bool FilterAccountDatas(object obj)
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

        private bool CanChangeAccountData(object arg)
        {
            return (arg as AccountData) != null;
        }

        private bool CanRemoveAccountData(object arg)
        {
            return (arg as AccountData) != null;
        }

        private void ClearFilteredText(object obj)
        {
            FilterText = string.Empty;
        }

        private void DeclineEdits(object obj)
        {
            if (IsEditMode.DoesAccountChange)
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
                if (IsEditMode.DoesAccountChange)
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
                //If new user
                if (new InputPassView().ShowDialog() == true)
                    FileProcess.Instance.WriteFile(obj as Account);
            }
            IsSaved = true;
        }

        private void ChangeAccountData(object obj)
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

        private void RemoveAccountData(object obj)
        {
            //Do you really want to delete it?
            var result = MessageBox.Show("Do you really want to delete this item?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        ThisAccount.Data.Remove((AccountData)obj);
                        IsSaved = false;
                        break;
                    }
                case MessageBoxResult.No:
                    {
                        break;
                    }
            }
        }

        private void AddAccountData(object obj)
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
