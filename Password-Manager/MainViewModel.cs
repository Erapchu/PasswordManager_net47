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

        public MainViewModel()
        {
            //Получение данных из файла
            AccountData[] account_data = FileProcess.ReadFile();

            //Инициализация
            IsEditMode = new EditMode(false, false);
            if (account_data != null) DataOfAccount = new ObservableCollection<AccountData>(account_data);
            else DataOfAccount = new ObservableCollection<AccountData>();
            FilteringCollection = CollectionViewSource.GetDefaultView(DataOfAccount);
            FilteringCollection.Filter = FilterAccounts;
        }

        AccountData ChangableAccount { get; set; }

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

        #region Commands of buttons
        public ICommand AddCommand
        {
            get
            {
                return new DelegateCommand(
                    (obj) =>
                    {
                        SelectedAccount = new AccountData();
                        IsEditMode.Switch(true);
                    });
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return new DelegateCommand(
                    (obj) =>
                    {
                        DataOfAccount.Remove((AccountData)obj);
                    },
                    (obj) =>
                    {
                        return (obj as AccountData) != null;
                    });
            }
        }

        public ICommand ChangeCommand
        {
            get
            {
                return new DelegateCommand(
                    (obj) =>
                    {
                        ChangableAccount = new AccountData {
                            Login = (obj as AccountData).Login, Name = (obj as AccountData).Name,
                            Other = (obj as AccountData).Other, Password = (obj as AccountData).Password
                        };
                        IsEditMode.Switch(true, true);
                    },
                    (obj) => 
                    {
                        return (obj as AccountData) != null;
                    });
            }
        }

        public ICommand SaveCommand { get; }

        public ICommand AcceptEditCommand
        {
            get
            {
                return new DelegateCommand(
                    (obj) =>
                    {
                        if(CheckEmptyInput())
                            if (IsEditMode.IsChange)
                            {
                                IsEditMode.Switch(false, false);
                            }
                            else
                            {
                                DataOfAccount.Add(new AccountData
                                {
                                    Login = SelectedAccount.Login, Name = SelectedAccount.Name,
                                    Other = SelectedAccount.Other, Password = SelectedAccount.Password
                                });
                                SelectedAccount = DataOfAccount.Last();
                                IsEditMode.Switch(false);
                            }
                    });
            }
        }

        public ICommand DeclineEditCommand
        {
            get
            {
                return new DelegateCommand(
                    (obj) =>
                    {
                        if (IsEditMode.IsChange)
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
                    });
            }
        }
        
        public ICommand ClearCommand
        {
            get
            {
                return new DelegateCommand(
                    (obj) =>
                    {
                        FilterText = string.Empty;
                    },
                    (obj) =>
                    {
                        return !string.IsNullOrEmpty(FilterText);
                    });
            }
        }
        #endregion

        #region MVVM Pattern
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        
        /*Старый способ:
        AddCommand = new DelegateCommand(AddAccount);
        private void AddAccount(object obj)
        {
            SelectedAccount = null;
            IsEditMode.Switch(true);
        }*/
    }
}
