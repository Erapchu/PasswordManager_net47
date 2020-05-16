using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Core.Data
{
    public class SortModes : ObservableCollection<SortMode>
    {
        public SortModes()
        {
            this.Add(new SortMode(SortType.DateAscending));
            this.Add(new SortMode(SortType.DateDescending));
            this.Add(new SortMode(SortType.NameAscending));
            this.Add(new SortMode(SortType.NameDescending));
        }
    }
}
