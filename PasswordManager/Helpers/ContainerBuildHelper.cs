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
        #region Singleton
        private static Lazy<ContainerBuildHelper> _lazy = new Lazy<ContainerBuildHelper>(() => null);
        public static ContainerBuildHelper Instance => _lazy.Value;
        #endregion

        public static bool IsInstanceInitialized { get; private set; }

        public static void InitializeInstance()
        {
            _lazy = new Lazy<ContainerBuildHelper>(() => new ContainerBuildHelper());
            IsInstanceInitialized = true;
        }

        private ContainerBuildHelper()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<MainWindow>();
            builder.RegisterType<InputPassWindow>();
            builder.RegisterType<IntroWindow>();

            builder.RegisterType<MainViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<InputPassViewModel>().InstancePerLifetimeScope();
            _lifetimeScope = builder.Build();
        }

        private readonly ILifetimeScope _lifetimeScope;

        public T Resolve<T>()
        {
            return _lifetimeScope.Resolve<T>();
        }

        public T Resolve<T>(params AutofacNamedParameter[] parameters)
        {
            return _lifetimeScope.Resolve<T>(parameters.Select(p => p.Parameter));
        }
    }
    public class AutofacNamedParameter
    {
        public NamedParameter Parameter { get; }
        public AutofacNamedParameter(string name, object value) => Parameter = new NamedParameter(name, value);
    }
}
