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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;

namespace PasswordManager.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        #region Design Time
        private static Lazy<MainViewModel> _lazy = new Lazy<MainViewModel>(() => new MainViewModel(IntPtr.Zero));
        public static MainViewModel DesignTimeInstance => _lazy.Value;

        private void LoadOnDesignTime()
        {
            var credentialsList = new List<Credentials>() 
            { 
                new Credentials("name", "login", "password", "other") 
                { 
                    LastTimeUsage = DateTime.Now,
                    CreationTime = DateTime.Today
                } 
            };
            CurrentAccount = new Account() { Credentials = new CredentialsCollection(credentialsList) };
            AllAccountsCollectionView = new ListCollectionView(CurrentAccount.Credentials) { Filter = FilterAccountDatas };
            SelectedCredentials = credentialsList.First();
            CurrentSortMode = SortModes.FirstOrDefault();
        }
        #endregion

        #region Fields
        private readonly IntPtr _windowHandle;
        #endregion

        #region Properties
        public ICollectionView AllAccountsCollectionView { get; private set; }

        public SortModes SortModes { get; } = new SortModes();

        public Account CurrentAccount { get; private set; }

        #region CurrentSortMode
        private SortMode _currentSortMode;
        public SortMode CurrentSortMode
        {
            get => _currentSortMode;
            set
            {
                _currentSortMode = value;
                RaisePropertyChanged();
                UpdateSorting();
                Task.Run(() =>
                {
                    Configuration.Instance.CurrentAccount.CredentialsSort = _currentSortMode.SortType;
                    Configuration.Instance.SaveAccount("Change sorting");
                });
            }
        }
        #endregion

        #region IsEditMode
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
        #endregion

        #region ChangableCredentials
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
        #endregion

        #region SelectedCredentials
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
        #endregion

        #region FilterText
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
                AllAccountsCollectionView?.Refresh();
                RaisePropertyChanged();
            }
        }
        #endregion
        #endregion

        #region Constructors
        public MainViewModel(IntPtr windowHandle)
        {
            this._windowHandle = windowHandle;

            if (this.IsInDesignMode)
                LoadOnDesignTime();
            else
            {
                CurrentAccount = Configuration.Instance.CurrentAccount;
                AllAccountsCollectionView = new ListCollectionView(CurrentAccount.Credentials) { Filter = FilterAccountDatas };
                SelectedCredentials = CurrentAccount.Credentials.FirstOrDefault();

                var savedSorting = Configuration.Instance.CurrentAccount.CredentialsSort;
                var sortMode = SortModes.FirstOrDefault(s => s.SortType.Equals(savedSorting));
                if (sortMode is null)
                    _currentSortMode = SortModes.FirstOrDefault();
                else
                    _currentSortMode = sortMode;
                UpdateSorting();
            }
        }
        #endregion

        #region Private methods
        private bool FilterAccountDatas(object obj)
        {
            bool result = true;
            if (obj is Credentials data && 
                !string.IsNullOrWhiteSpace(FilterText) && 
                !data.Name.ToLower().Contains(FilterText)) 
                return false;
            return result;
        }

        private bool CheckEmptyInput()
        {
            if (string.IsNullOrEmpty(SelectedCredentials.Login) || string.IsNullOrEmpty(SelectedCredentials.Name) || string.IsNullOrEmpty(SelectedCredentials.Password)) return false;
            else return true;
        }

        private void UpdateSorting()
        {
            switch (CurrentSortMode.SortType)
            {
                case SortType.NameAscending:
                    CurrentAccount.Credentials.Sort(i => i.Name);
                    break;
                case SortType.DateAscending:
                    CurrentAccount.Credentials.Sort(i => i.LastTimeUsage);
                    break;
                case SortType.NameDescending:
                    CurrentAccount.Credentials.SortDescending(i => i.Name);
                    break;
                case SortType.DateDescending:
                    CurrentAccount.Credentials.SortDescending(i => i.LastTimeUsage);
                    break;
            }
        }

        private void DeclineEdits()
        {
            if (ChangableCredentials is null)
            {
                SelectedCredentials = CurrentAccount.Credentials.FirstOrDefault();
            }
            else
            {
                SelectedCredentials = ChangableCredentials;
                CurrentAccount.Credentials[SelectedCredentials.ID] = ChangableCredentials;
            }
            IsEditMode = false;
            ChangableCredentials = null;
            UpdateCommandState();
        }

        private void AcceptEdits()
        {
            if (CheckEmptyInput())
            {
                SelectedCredentials.LastTimeUsage = DateTime.Now;
                //If add new account
                if (ChangableCredentials is null)
                {
                    CurrentAccount.Credentials.Add(SelectedCredentials);
                }

                //Clear changable account
                ChangableCredentials = null;
                IsEditMode = false;
                UpdateSorting();
                Configuration.Instance.SaveAccount("Credentials was add/changed");
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
                CurrentAccount.Credentials.Remove(SelectedCredentials);
                Configuration.Instance.SaveAccount("Credential was deleted");
            }
            UpdateCommandState();
        }

        private void AddAccountData()
        {
            SelectedCredentials = new Credentials() 
            { 
                CreationTime = DateTime.Now,
                LastTimeUsage = DateTime.Now
            };
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

            if (CurrentAccount.Credentials.Count == 0)
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
