using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PasswordManager.Core.Data;
using PasswordManager.Core.Encryption;
using PasswordManager.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        private void LoadOnDesignTime()
        {
            StatusText = "Design time";
            IsStatusShowed = true;
        }

        #region Constructors
        public InputPassViewModel()
        {
            if (this.IsInDesignMode)
                LoadOnDesignTime();
            else
            {
                if (Configuration.Instance.CurrentAccount.CorrectPassword is null)
                {
                    StatusText = "Please, enter your new password";
                    IsStatusShowed = true;
                }
            }
        }
        #endregion

        #region Implements of commands
        private void Continue(object obj)
        {
            PasswordBox passwordBox = obj as PasswordBox;
            var encryptedInputedPassword = TripleDESHelper.EncryptString(passwordBox.Password);

            var correctPassword = Configuration.Instance.CurrentAccount.CorrectPassword;
            if (correctPassword is null)
            {
                Configuration.Instance.CurrentAccount.SetNewPassword(encryptedInputedPassword);
                ContinueAuthorization?.Invoke();
                return;
            }

            if (encryptedInputedPassword.Equals(correctPassword))
            {
                Logger.Info("Password is correct.");
                ContinueAuthorization?.Invoke();
                return;
            }
            else
            {
                Logger.Warn("Password is incorrect.");
                StatusText = "Password is incorrect";
                IsStatusShowed = true;
            }
        }
        #endregion

        #region Commands
        private RelayCommand<object> continueCommand;
        public RelayCommand<object> ContinueCommand => continueCommand
            ?? (continueCommand = new RelayCommand<object>(Continue));
        #endregion
    }
}
