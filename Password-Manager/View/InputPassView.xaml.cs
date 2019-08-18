using Password_Manager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Password_Manager.View
{
    /// <summary>
    /// Interaction logic for PasswordView.xaml
    /// </summary>
    public partial class InputPassView : Window
    {
        internal InputPassViewModel inputPassViewModel; 
        public InputPassView()
        {
            InitializeComponent();
        }

        public InputPassView(string password, PassOperation operation): this()
        {
            if (operation == PassOperation.ChangePassword || operation == PassOperation.DefaultUser)
                inputPassViewModel = new InputPassViewModel(password, operation);
            if (operation == PassOperation.NewUser)
                inputPassViewModel = new InputPassViewModel();
            this.DataContext = inputPassViewModel;
        }
    }
}
