using PasswordManager.Helpers;
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
    internal class InputPassViewModel : INotifyPropertyChanged
    {
        #region Fields
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            } 
        }
        #endregion

        #region Constructors
        public InputPassViewModel()
        {
            ContinueCommand = new DelegateCommand(Continue, CanContinue);
            ExitCommand = new DelegateCommand(Exit);
            statusVisibility = Visibility.Collapsed;
        }

        public InputPassViewModel(string pass, PassOperation op): this()
        {
            this.Operation = op;
            this.CorrectPassword = pass;
        }
        #endregion

        #region Implements of commands
        private void Exit(object obj)
        {
            Environment.Exit(0);
        }

        private bool CanContinue(object arg)
        {
            return !string.IsNullOrWhiteSpace(Password);
        }

        private void Continue(object obj)
        {
            Window window = obj as Window;
            if (window == null) return;

            switch (this.Operation)
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
            }
        }
        #endregion

        #region Commands
        public ICommand ContinueCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }
        #endregion

        #region MVVM Pattern
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
