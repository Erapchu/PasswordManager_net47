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
        #region Fields
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                RaisePropertyChanged();
            }
        }

        public string CorrectPassword { get; private set; }
        public PassOperation Operation { get; private set; }

        private string status;
        public string Status 
        { 
            get
            {
                return status;
            }
            private set
            {
                status = value;
                RaisePropertyChanged();
            } 
        }

        private Visibility statusVisibility;
        public Visibility StatusVisibility 
        { 
            get 
            { 
                return statusVisibility; 
            }
            set 
            {
                statusVisibility = value;
                RaisePropertyChanged();
            } 
        }
        #endregion

        #region Constructors
        public InputPassViewModel()
        {
            //statusVisibility = Visibility.Collapsed;
        }
        #endregion

        #region Implements of commands
        private void Exit()
        {
            //Environment.Exit(0);
        }

        private void Continue(Window window)
        {
            if (window == null) return;

            /*switch (this.Operation)
            {
                case PassOperation.DefaultUser:
                    if (!string.IsNullOrWhiteSpace(CorrectPassword) && CorrectPassword == Password)
                        window.DialogResult = true;
                    else
                    {
                        StatusVisibility = Visibility.Visible;
                        Status = "Password is incorrect";
                    }
                    break;
                case PassOperation.ChangePassword:
                    if (CorrectPassword != Password)
                    {
                        CorrectPassword = Password;
                        window.DialogResult = true;
                    }
                    else 
                        MessageBox.Show("New password is equivalent old password", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case PassOperation.NewUser:
                    CorrectPassword = Password;
                    window.DialogResult = true;
                    break;
            }*/
        }
        #endregion

        #region Commands
        private RelayCommand<Window> continueCommand;
        public RelayCommand<Window> ContinueCommand => continueCommand
            ?? (continueCommand = new RelayCommand<Window>(Continue));

        private RelayCommand exitCommand;
        public RelayCommand ExitCommand => exitCommand
            ?? (exitCommand = new RelayCommand(Exit));
        #endregion
    }
}
