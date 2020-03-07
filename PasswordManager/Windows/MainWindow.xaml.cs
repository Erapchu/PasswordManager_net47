﻿using Autofac;
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
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;
        ILifetimeScope _lifetimeScope;
        public MainWindow(ILifetimeScope lifetimeScope)
        {
            InitializeComponent();
            _lifetimeScope = lifetimeScope.BeginLifetimeScope();
            viewModel = _lifetimeScope.Resolve<MainViewModel>();
            DataContext = viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO Start reading file
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _lifetimeScope.Dispose();
            _lifetimeScope = null;
        }
    }
}
