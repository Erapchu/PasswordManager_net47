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
        public EditMode(bool forView, bool ischange)
        {
            IsReadOnly = !forView;
            IsEnabled = forView;
            LeftPanel = !forView;
            IsChange = ischange;
        }

        public void Switch(bool forView, bool ischange)
        {
            IsReadOnly = !forView;
            IsEnabled = forView;
            LeftPanel = !forView;
            IsChange = ischange;
        }
        public void Switch(bool forView)
        {
            IsReadOnly = !forView;
            IsEnabled = forView;
            LeftPanel = !forView;
        }

        private bool _IsChange;
        public bool IsChange
        {
            get
            {
                return _IsChange;
            }
            set
            {
                _IsChange = value;
                OnPropertyChanged();
            }
        }

        private bool _LeftPanel;
        public bool LeftPanel
        {
            get
            {
                return _LeftPanel;
            }
            set
            {
                _LeftPanel = value;
                OnPropertyChanged();
            }
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
