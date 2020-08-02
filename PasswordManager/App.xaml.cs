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
        #region Private fields
        IntroWindow _introWindow;
        InputPassWindow _inputPassWindow;
        MainWindow _mainWindow;
        #endregion

        #region Constructors
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        #endregion

        #region Events
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            Logger.Error($"[Unhandled] {exception}");
        }

        void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Configuration.Instance?.Save();
            Logger.Info("Leaving application\r\n");
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeComponent();
            System.Windows.Forms.Application.EnableVisualStyles();

            Logger.InitLogger(Constants.PathToLoggerFile);
            Logger.Info("Log session started!");

            try
            {
                //Create IoC here
                ContainerBuildHelper.InitializeInstance();

                _introWindow = ContainerBuildHelper.Instance.Resolve<IntroWindow>();
                _introWindow.Show();

                var readConfigurationTaskResult = await Task.Run(() => Configuration.InitializeConfiguration());
                if (!readConfigurationTaskResult)
                {
                    Logger.Warn("Can't initialize Configuration instance.");
                    this.Shutdown();
                }

                _inputPassWindow = ContainerBuildHelper.Instance.Resolve<InputPassWindow>();
                _mainWindow = ContainerBuildHelper.Instance.Resolve<MainWindow>();

                _introWindow.Close();
                _introWindow = null;

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
                Logger.Error(ex.Message);
                this.Shutdown();
            }
        }
        #endregion
    }
}
