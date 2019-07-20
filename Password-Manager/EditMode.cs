using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager
{
    class EditMode : INotifyPropertyChanged
    {
        public EditMode(bool b)
        {
            IsReadOnly = !b;
            IsEnabled = b;
        }

        public void Switch(bool b)
        {
            IsReadOnly = !b;
            IsEnabled = b;
        }

        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }
            private set
            {
                _IsReadOnly = value;
                OnPropertyChanged();
            }
        }
        private bool _IsEnabled;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                _IsEnabled = value;
                OnPropertyChanged();
            }
        }

        #region MVVM Pattern
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
