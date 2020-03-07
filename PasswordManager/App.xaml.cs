using PasswordManager.Core.Data;
using PasswordManager.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordManager
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        ContainerBuildHelper _buildHelper;
        InputPassWindow _inputPassWindow;
        MainWindow _mainWindow;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeComponent();

            Logger.Info("Log session started!");

            //Start reading file
            var config = Configuration.Instance;

            //Create IoC here
            _buildHelper = new ContainerBuildHelper();
            _inputPassWindow = _buildHelper.Resolve<InputPassWindow>();
            _inputPassWindow.ShowDialog();

            //_mainWindow = _buildHelper.Resolve<MainWindow>();
            //_mainWindow.Show();
        }
    }
}
