using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager
{
    class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<AccountData> AccountDatas { get; private set; }
        public MainViewModel()
        {
            AccountDatas = new ObservableCollection<AccountData>(AccountData.ReadFile());
        }



        #region MVVM Pattern
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string property_name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
        }
        #endregion
    }
}
