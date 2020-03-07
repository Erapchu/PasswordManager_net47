using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PasswordManager.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PasswordManager.ViewModel
{
    internal class InputPassViewModel : ViewModelBase
    {
        #region Design Time Instance
        private static Lazy<InputPassViewModel> _lazy = new Lazy<InputPassViewModel>(() => new InputPassViewModel());
        public static InputPassViewModel DesignTimeInstance => _lazy.Value;
        #endregion

        #region Properties
        public event Action ContinueAuthorization;

        private string currentPassword;
        public string CurrentPassword
        {
            get => currentPassword;
            set
            {
                currentPassword = value;
                RaisePropertyChanged();
            }
        }

        public string CorrectPassword { get; private set; }
        public PassOperation Operation { get; private set; }

        private string statusText;
        public string StatusText
        {
            get => statusText;
            private set
            {
                statusText = value;
                RaisePropertyChanged();
            } 
        }

        private bool isStatusShowed;
        public bool IsStatusShowed
        {
            get => isStatusShowed;
            set 
            {
                isStatusShowed = value;
                RaisePropertyChanged();
            } 
        }
        #endregion

        #region Constructors
        public InputPassViewModel()
        {

        }
        #endregion

        #region Implements of commands
        private void Continue()
        {
            if (CorrectPassword is null)
            {
                StatusText = "Can't load correct password";
                IsStatusShowed = true;
                return;
            }

            if (CurrentPassword.Equals(CorrectPassword))
                ContinueAuthorization?.Invoke();
            else
            {
                StatusText = "Password is incorrect";
                IsStatusShowed = true;
            }
        }
        #endregion

        #region Commands
        private RelayCommand continueCommand;
        public RelayCommand ContinueCommand => continueCommand
            ?? (continueCommand = new RelayCommand(Continue));
        #endregion
    }
}
