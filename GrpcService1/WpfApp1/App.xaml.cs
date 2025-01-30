using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Grpc.Net.Client;
using WpfApp1.Data;

namespace WpfApp1
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // Настройка gRPC канала
            var channel = GrpcChannel.ForAddress("http://localhost:5000");
            services.AddSingleton(channel);

            // Регистрация сервисов
            services.AddSingleton<IClientService, ClientService>();

            // Регистрация окна через DI
            services.AddSingleton<MainWindow>();

            // Создание сервисов
            ServiceProvider = services.BuildServiceProvider();

            // Запуск приложения через DI
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}