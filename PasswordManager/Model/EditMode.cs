using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    class EditMode : INotifyPropertyChanged
    {
        public EditMode(bool forView, bool isChange)
        {
            IsReadOnly = !forView;
            IsEnabled = forView;
            LeftPanel = !forView;
            DoesAccountChange = isChange;
        }

        /// <summary>
        /// Switch mode to "Edit" (isChange parameter) or "View datas"
        /// </summary>
        /// <param name="forView">IsReadOnly, IsEnabled properties for View must be opposite</param>
        /// <param name="isChange">Does account change now</param>
        public void Switch(bool forView, bool isChange)
        {
            IsReadOnly = !forView;
            IsEnabled = forView;
            LeftPanel = !forView;
            DoesAccountChange = isChange;
        }

        /// <summary>
        /// Switch mode to "Add" or "View datas"
        /// </summary>
        /// <param name="forView">IsReadOnly, IsEnabled properties for View must be opposite</param>
        public void Switch(bool forView)
        {
            IsReadOnly = !forView;
            IsEnabled = forView;
            LeftPanel = !forView;
        }

        private bool _doesAccontChange;
        public bool DoesAccountChange
        {
            get
            {
                return _doesAccontChange;
            }
            set
            {
                _doesAccontChange = value;
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
