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
            inputPassViewModel = new InputPassViewModel();
            this.DataContext = inputPassViewModel;
        }

        public InputPassView(string password, PassOperation operation): this()
        {
            switch (operation)
            {
                case PassOperation.ChangePassword:
                    inputPassViewModel.CorrectPassword = password;
                    break;
                case PassOperation.DefaultUser:
                    inputPassViewModel.CorrectPassword = password;
                    break;
                case PassOperation.NewUser:
                    break;
                default: break;
            }
        }
    }
}
