using PasswordManager.Core;
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

            Logger.SetPathToLogger(Constants.PathToLogger);
            Logger.Instance.Info("Log session started!");

            Logger.Instance.Info("Start reading configuration...");
            Task.Run(() => Configuration.Instance);

            //Create IoC here
            _buildHelper = new ContainerBuildHelper();

            //Create login window
            Logger.Instance.Info("Initialize login window...");
            _inputPassWindow = _buildHelper.Resolve<InputPassWindow>();
            _inputPassWindow.ShowDialog();

            if (_inputPassWindow.DialogResult == true)
            {
                _mainWindow = _buildHelper.Resolve<MainWindow>();
                _mainWindow.Show();
            }
        }
    }
}
