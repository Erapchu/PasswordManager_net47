using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Password_Manager.ViewModel
{
    internal class InputPassViewModel : INotifyPropertyChanged
    {
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string CorrectPassword { get; set; }

        public InputPassViewModel()
        {
            ContinueCommand = new DelegateCommand(Continue, CanContinue);
            ExitCommand = new DelegateCommand(Exit);
        }

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

            if (CorrectPassword != null && CorrectPassword == Password) window.DialogResult = true;
            else
            {
                CorrectPassword = Password;
                window.DialogResult = true;
            }
        }

        public ICommand ContinueCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        #region MVVM Pattern
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
