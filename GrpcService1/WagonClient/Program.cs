using Grpc.Net.Client;
using GrpcWagonService;  // Это пространство имен, соответствующее вашему .proto файлу
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Создание канала для общения с сервером
        var channel = GrpcChannel.ForAddress("http://localhost:5000"); // Замените на адрес вашего сервера

        // Создание клиента
        var client = new WagonService.WagonServiceClient(channel);

        // Установка времени начала и конца
        var startTime = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-1)); // 1 день назад
        var endTime = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(1));   // 1 день вперед

        // Создание запроса
        var request = new WagonRequest
        {
            StartTime = startTime,
            EndTime = endTime
        };

        // Выполнение запроса
        var response = await client.GetWagonsAsync(request);

        // Вывод информации о вагонах
        foreach (var wagon in response.Wagons)
        {
            Console.WriteLine($"Инвентарный номер: {wagon.InventoryNumber}, " +
                              $"Прибытие: {wagon.ArrivalTime.ToDateTime()}, " +
                              $"Отправление: {wagon.DepartureTime.ToDateTime()}");
        }

        Console.WriteLine("Запрос выполнен.");
    }
}