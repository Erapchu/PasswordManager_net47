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
            var credentialsList = new List<Credentials>() { new Credentials("name", "login", "password", "other") };
            ThisAccount = new Account() { Data = new CredentialsCollection(credentialsList) };
            AllAccountsCollectionView = new ListCollectionView(ThisAccount.Data) { Filter = FilterAccountDatas };
            SelectedCredentials = credentialsList.First();
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
            }
        }

        private Credentials _changableAcountData;
        public Credentials ChangableCredentials
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

        private Credentials _selectedCredentials;
        public Credentials SelectedCredentials
        {
            get
            {
                return _selectedCredentials;
            }
            set
            {
                _selectedCredentials = value;
                RaisePropertyChanged();
            }
        }

        private string _filterText;
        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                _filterText = value;
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
                SelectedCredentials = ThisAccount.Data.FirstOrDefault();
            }
        }

        private bool FilterAccountDatas(object obj)
        {
            bool result = true;
            Credentials data = obj as Credentials;
            if (data != null && !string.IsNullOrWhiteSpace(FilterText) && !data.Name.Contains(FilterText)) return false;
            return result;
        }

        private bool CheckEmptyInput()
        {
            if (string.IsNullOrEmpty(SelectedCredentials.Login) || string.IsNullOrEmpty(SelectedCredentials.Name) || string.IsNullOrEmpty(SelectedCredentials.Password)) return false;
            else return true;
        }

        #region Delegate commands

        private void DeclineEdits()
        {
            if (ChangableCredentials is null)
            {
                SelectedCredentials = ThisAccount.Data.FirstOrDefault();
            }
            else
            {
                SelectedCredentials = ChangableCredentials;
                ThisAccount.Data[SelectedCredentials.ID] = ChangableCredentials;
            }
            IsEditMode = false;
            ChangableCredentials = null;
            UpdateCommandState();
        }

        private void AcceptEdits()
        {
            if (CheckEmptyInput())
            {
                //If add new account
                if (ChangableCredentials is null)
                    ThisAccount.Data.Add(SelectedCredentials);

                //Clear changable account
                ChangableCredentials = null;
                IsEditMode = false;
                Configuration.Instance.SaveData();
                UpdateCommandState();
            }
            else
                System.Windows.Forms.MessageBox.Show(
                    "Please, fill this data: Name, Login, Password", 
                    "Information",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information,
                    System.Windows.Forms.MessageBoxDefaultButton.Button1);
        }

        private void ChangeAccountData()
        {
            ChangableCredentials = SelectedCredentials.Clone() as Credentials;
            IsEditMode = true;
            UpdateCommandState();
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
                ThisAccount.Data.Remove(SelectedCredentials);
                Configuration.Instance.SaveData();
            }
            UpdateCommandState();
        }

        private void AddAccountData()
        {
            SelectedCredentials = new Credentials();
            IsEditMode = true;
            UpdateCommandState();
        }

        private void UpdateCommandState()
        {
            AddCommand.RaiseCanExecuteChanged();
            ChangeCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
        }

        private bool CanManipulateWithCredentials()
        {
            if (IsEditMode)
                return false;

            if (ThisAccount.Data.Count == 0)
                return false;

            return true;
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
                    ?? (_removeCommand = new RelayCommand(RemoveAccountData, CanManipulateWithCredentials));
            }
        }

        private RelayCommand _changeCommand;
        public RelayCommand ChangeCommand
        {
            get
            {
                return _changeCommand
                    ?? (_changeCommand = new RelayCommand(ChangeAccountData, CanManipulateWithCredentials));
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
