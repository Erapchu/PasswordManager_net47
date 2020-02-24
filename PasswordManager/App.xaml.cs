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
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeComponent();
            //Start reading file

            Logger.Info("Log session started!");

            //TODO: Create IoC here

            //Start Views
            StartupUri = new Uri("Windows/MainWindow.xaml", UriKind.RelativeOrAbsolute);
        }

        private void CreateIoC()
        {

        }
    }
}
