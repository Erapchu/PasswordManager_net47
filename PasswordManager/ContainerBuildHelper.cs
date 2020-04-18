using Autofac;
using PasswordManager.ViewModel;
using PasswordManager.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    public class ContainerBuildHelper
    {
        ILifetimeScope _lifetimeScope;

        public ContainerBuildHelper()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<MainWindow>();
            builder.RegisterType<InputPassWindow>();
            builder.RegisterType<IntroWindow>();

            builder.RegisterType<MainViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<InputPassViewModel>().InstancePerLifetimeScope();
            _lifetimeScope = builder.Build();
        }

        public T Resolve<T>()
        {
            return _lifetimeScope.Resolve<T>();
        }
    }
}
