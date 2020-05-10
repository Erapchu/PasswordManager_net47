using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PasswordManager.Core.Data;
using PasswordManager.Core.Helpers;
using PasswordManager.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;

namespace PasswordManager.ViewModel
{
    public enum PassOperation
    {
        NewUser,
        DefaultUser,
        ChangePassword
    }

    class MainViewModel : ViewModelBase
    {
        #region Design Time
        private static Lazy<MainViewModel> _lazy = new Lazy<MainViewModel>(() => new MainViewModel(IntPtr.Zero));
        public static MainViewModel DesignTimeInstance => _lazy.Value;

        private void LoadOnDesignTime()
        {
            var accountDataList = new List<AccountData>() { new AccountData("name", "login", "password", "other") };
            ThisAccount = new Account() { Data = new ObservableCollection<AccountData>(accountDataList) };
            AllAccountsCollectionView = new ListCollectionView(ThisAccount.Data) { Filter = FilterAccountDatas };
            SelectedAccountData = accountDataList.First();
        }
        #endregion

        public IntPtr _windowHandle;

        public ICollectionView AllAccountsCollectionView { get; private set; }

        private bool _isEditMode;
        public bool IsEditMode 
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                RaisePropertyChanged();

                AddCommand.RaiseCanExecuteChanged();
                ChangeCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            }
        }

        private AccountData _changableAcountData;
        public AccountData ChangableAccountData
        {
            get
            {
                return _changableAcountData;
            }
            set
            {
                _changableAcountData = value;
                RaisePropertyChanged();
            }
        }

        private AccountData _selectedAccountData;
        public AccountData SelectedAccountData
        {
            get
            {
                return _selectedAccountData;
            }
            set
            {
                _selectedAccountData = value;
                RaisePropertyChanged();
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
                AllAccountsCollectionView.Refresh();
                RaisePropertyChanged();
            }
        }

        public Account ThisAccount { get; private set; }

        public MainViewModel(IntPtr windowHandle)
        {
            this._windowHandle = windowHandle;

            if (this.IsInDesignMode)
                LoadOnDesignTime();
            else
            {
                ThisAccount = Configuration.Instance.CurrentAccount;
                AllAccountsCollectionView = new ListCollectionView(ThisAccount.Data) { Filter = FilterAccountDatas };
                SelectedAccountData = ThisAccount.Data.FirstOrDefault();
            }
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
            if (string.IsNullOrEmpty(SelectedAccountData.Login) || string.IsNullOrEmpty(SelectedAccountData.Name) || string.IsNullOrEmpty(SelectedAccountData.Password)) return false;
            else return true;
        }

        #region Delegate commands

        private void DeclineEdits()
        {
            ThisAccount.Data[ThisAccount.Data.IndexOf(SelectedAccountData)] = ChangableAccountData;
            SelectedAccountData = ChangableAccountData;
            IsEditMode = false;
        }

        private void AcceptEdits()
        {
            if (CheckEmptyInput())
            {
                ThisAccount.Data.Add(new AccountData(
                    SelectedAccountData.Name,
                    SelectedAccountData.Login,
                    SelectedAccountData.Password,
                    SelectedAccountData.Other ?? string.Empty));
                SelectedAccountData = ThisAccount.Data.Last();
                IsEditMode = false;
                Configuration.Instance.SaveData();
            }
            else MessageBox.Show("Please, fill this data: Name, Login, Password", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ChangeAccountData()
        {
            ChangableAccountData = new AccountData(
                    SelectedAccountData.Name,
                    SelectedAccountData.Login,
                    SelectedAccountData.Password,
                    SelectedAccountData.Other ?? string.Empty);
            IsEditMode = true;
        }

        private void RemoveAccountData()
        {
            //Do you really want to delete it?
            var result = System.Windows.Forms.MessageBox.Show(
                new HwndWrapper(_windowHandle),
                "Do you really want to delete this item?", 
                "Question",
                System.Windows.Forms.MessageBoxButtons.YesNo,
                System.Windows.Forms.MessageBoxIcon.Question,
                System.Windows.Forms.MessageBoxDefaultButton.Button1);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                ThisAccount.Data.Remove(SelectedAccountData);
                Configuration.Instance.SaveData();
            }
        }

        private void AddAccountData()
        {
            SelectedAccountData = new AccountData();
            IsEditMode = true;
        }
        #endregion

        #region Commands of buttons
        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand
                    ?? (_addCommand = new RelayCommand(AddAccountData, () => !IsEditMode));
            }
        }

        private RelayCommand _removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand
                    ?? (_removeCommand = new RelayCommand(RemoveAccountData, () => !IsEditMode));
            }
        }

        private RelayCommand _changeCommand;
        public RelayCommand ChangeCommand
        {
            get
            {
                return _changeCommand
                    ?? (_changeCommand = new RelayCommand(ChangeAccountData, () => !IsEditMode));
            }
        }

        private RelayCommand _acceptEditCommand;
        public RelayCommand AcceptEditCommand
        {
            get
            {
                return _acceptEditCommand
                    ?? (_acceptEditCommand = new RelayCommand(AcceptEdits));
            }
        }

        private RelayCommand _declineEditCommand;
        public RelayCommand DeclineEditCommand
        {
            get
            {
                return _declineEditCommand
                    ?? (_declineEditCommand = new RelayCommand(DeclineEdits));
            }
        }
        #endregion
    }
}
