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
        IntroWindow _introWindow;
        InputPassWindow _inputPassWindow;
        MainWindow _mainWindow;

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeComponent();
            System.Windows.Forms.Application.EnableVisualStyles();

            Logger.SetPathToLogger(Constants.PathToLogger);
            Logger.Instance.Info("Log session started!");

            try
            {
                //Create IoC here
                ContainerBuildHelper.InitializeInstance();

                Logger.Instance.Info("Start reading configuration...");

                _introWindow = ContainerBuildHelper.Instance.Resolve<IntroWindow>();
                _introWindow.Show();

                await Task.Run(() => Configuration.InitializeConfiguration());
                _inputPassWindow = ContainerBuildHelper.Instance.Resolve<InputPassWindow>();
                _mainWindow = ContainerBuildHelper.Instance.Resolve<MainWindow>();

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
                Current.Shutdown();
            }
        }
    }
}
