using Microsoft.Extensions.DependencyInjection;
using Grpc.Net.Client;
using GrpcWagonService;
using WagonClient.Data;
using Google.Protobuf.WellKnownTypes;

class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton(GrpcChannel.ForAddress("http://localhost:5000"))
            .AddSingleton<WagonService.WagonServiceClient>(sp => 
                new WagonService.WagonServiceClient(sp.GetRequiredService<GrpcChannel>()))
            .AddSingleton<IWagonServiceClient, WagonServiceClient>()
            .BuildServiceProvider();

        var wagonService = serviceProvider.GetRequiredService<IWagonServiceClient>();

        // Запрос времени
        Console.WriteLine("Введите дату начала (yyyy-MM-dd HH:mm):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDateTime))
        {
            Console.WriteLine("Некорректный формат даты.");
            return;
        }

        Console.WriteLine("Введите дату окончания (yyyy-MM-dd HH:mm):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDateTime))
        {
            Console.WriteLine("Некорректный формат даты.");
            return;
        }

        // Преобразование в Timestamp
        var startTime = Timestamp.FromDateTime(startDateTime.ToUniversalTime());
        var endTime = Timestamp.FromDateTime(endDateTime.ToUniversalTime());

        // Выполнение запроса через DI-сервис
        var response = await wagonService.GetWagonsAsync(startTime, endTime);

        // Вывод результата
        foreach (var wagon in response.Wagons)
        {
            Console.WriteLine($"Инвентарный номер: {wagon.InventoryNumber}, " +
                              $"Прибытие: {(wagon.ArrivalTime != null ? wagon.ArrivalTime.ToDateTime() : "не указано")}, " +
                              $"Отправление: {(wagon.DepartureTime != null ? wagon.DepartureTime.ToDateTime() : "не указано")}");
        }

        Console.WriteLine("Запрос выполнен.");
    }
}
