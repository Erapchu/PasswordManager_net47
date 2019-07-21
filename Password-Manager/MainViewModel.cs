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
        public EditMode IsEditMode { get; set; }

        public MainViewModel()
        {
            //Получение данных из файла
            AccountData[] account_data = File_process.ReadFile();
            if (account_data != null) DataOfAccount = new ObservableCollection<AccountData>(account_data);
            else DataOfAccount = new ObservableCollection<AccountData>();

            //Инициализация
            IsEditMode = new EditMode(false, false);
        }

        public AccountData ChangableAccount { get; set; }

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
                            Login = SelectedAccount.Login, Name = SelectedAccount.Name,
                            Other = SelectedAccount.Other, Password = SelectedAccount.Password
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
                        //додумать
                        if (IsEditMode.IsChange)
                        {
                            SelectedAccount = ChangableAccount;
                        }
                        SelectedAccount = null;
                        if (IsEditMode.IsChange)
                            IsEditMode.Switch(false, false);
                        else
                            IsEditMode.Switch(false);
                    });
            }
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

        /*Старый способ:
        //AddCommand = new DelegateCommand(AddAccount);
        private void AddAccount(object obj)
        {
            SelectedAccount = null;
            IsEditMode.Switch(true);
        }*/

        #region MVVM Pattern
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
