using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ALRC.Creator.Extensions;
using ALRC.Creator.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace ALRC.Creator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherExtension.UiThreadId = Dispatcher.Thread.ManagedThreadId;
            DispatcherExtension.Dispatcher = Dispatcher;
            base.OnStartup(e);
            ConfigureServices();
            this.AddConverters();
            DI.Services.GetRequiredService<MainWindow>().Show();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddMvvm();
            DI.Services = services.BuildServiceProvider();
        }
    }
}