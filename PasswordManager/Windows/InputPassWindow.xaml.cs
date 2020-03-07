using Autofac;
using PasswordManager.ViewModel;
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

namespace PasswordManager.Windows
{
    /// <summary>
    /// Interaction logic for PasswordView.xaml
    /// </summary>
    public partial class InputPassWindow : Window
    {
        private InputPassViewModel viewModel;
        private ILifetimeScope _lifetimeScope;
        public InputPassWindow(ILifetimeScope lifetimeScope)
        {
            InitializeComponent();

            _lifetimeScope = lifetimeScope.BeginLifetimeScope();
            viewModel = _lifetimeScope.Resolve<InputPassViewModel>();
            viewModel.ContinueAuthorization += ViewModel_ContinueAuthorization;
            DataContext = viewModel;
        }

        private void ViewModel_ContinueAuthorization()
        {
            DialogResult = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _lifetimeScope.Dispose();
            _lifetimeScope = null;
        }
    }
}
