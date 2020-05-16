using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Core.Data
{

    public class SortMode : INotifyPropertyChanged
    {
        #region Constructors
        public SortMode(SortType type)
        {
            this.SortType = type;
        }
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                switch(SortType)
                {
                    case SortType.NameAscending:
                        return "Name (ascending)";
                    case SortType.NameDescending:
                        return "Name (descending)";
                    case SortType.DateAscending:
                        return "Date (ascending)";
                    case SortType.DateDescending:
                        return "Date (descending)";
                }
                return "None";
            }
        }

        private SortType _sortType;
        public SortType SortType
        {
            get => _sortType;
            private set
            {
                _sortType = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            string output = this.Name;
            return output;
        }
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
