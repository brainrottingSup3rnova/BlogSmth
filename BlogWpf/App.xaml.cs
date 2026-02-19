using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using Infrastructure.Repositories;
using Application.Interfaces;
using Application.UseCases;
using BlogWpf.ViewModels;

namespace BlogWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private IHost? _host;

        protected override async void OnStartup(StartupEventArgs e)
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => 
                {
                    services.AddSingleton<IBlogRepository>(sp => new JsonBlogRepository());
                    services.AddSingleton<IBlogService, BlogService>();
                    services.AddSingleton<HomeViewModel>();
                    services.AddSingleton<MainWindow>();
                }).Build();

            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if(_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            base.OnExit(e);
        }
    }
}
