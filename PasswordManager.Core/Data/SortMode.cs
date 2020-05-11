using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PasswordManager.Core.Data
{
    public enum SortType
    {
        NameAscending,
        NameDescending,
        DateAscending,
        DateDescending
    }

    public class SortMode : INotifyPropertyChanged
    {
        public SortMode(SortType type)
        {
            this.SortType = type;
        }

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

        private BitmapImage _image;
        public BitmapImage Image
        {
            get => _image;
            private set
            {
                _image = value;
                OnPropertyChanged();
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

        public override string ToString()
        {
            string output = this.Name;
            return output;
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
