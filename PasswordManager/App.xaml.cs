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

        IntroWindow _introWindow;
        InputPassWindow _inputPassWindow;
        MainWindow _mainWindow;

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeComponent();

            Logger.SetPathToLogger(Constants.PathToLogger);
            Logger.Instance.Info("Log session started!");

            //Create IoC here
            try
            {
                _buildHelper = new ContainerBuildHelper();

                Logger.Instance.Info("Start reading configuration...");

                _introWindow = _buildHelper.Resolve<IntroWindow>();
                _introWindow.Show();

                await Task.Run(() => Configuration.InitializeConfiguration());
                _inputPassWindow = _buildHelper.Resolve<InputPassWindow>();
                _mainWindow = _buildHelper.Resolve<MainWindow>();

                _introWindow.Close();
                _introWindow = null;

                Logger.Instance.Info("Initialize login window...");
                _inputPassWindow.ShowDialog();

                if (_inputPassWindow.DialogResult == true)
                {
                    _mainWindow.Show();
                }
                else
                {
                    _mainWindow.Close();
                    _mainWindow = null;
                }
                _inputPassWindow.Close();
                _inputPassWindow = null;

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);

                _introWindow.Close();
                _introWindow = null;
                _mainWindow.Close();
                _mainWindow = null;
                _inputPassWindow.Close();
                _inputPassWindow = null;
            }
        }
    }
}
