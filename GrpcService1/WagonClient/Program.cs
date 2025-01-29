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

        // Запрос времени начала
        Console.WriteLine("Введите дату начала (в формате: yyyy-MM-dd HH:mm):");
        string startInput = Console.ReadLine();
        DateTime startDateTime;
        if (!DateTime.TryParse(startInput, out startDateTime))
        {
            Console.WriteLine("Некорректный формат даты начала.");
            return;
        }
        
        // Запрос времени окончания
        Console.WriteLine("Введите дату окончания (в формате: yyyy-MM-dd HH:mm):");
        string endInput = Console.ReadLine();
        DateTime endDateTime;
        if (!DateTime.TryParse(endInput, out endDateTime))
        {
            Console.WriteLine("Некорректный формат даты окончания.");
            return;
        }

        // Преобразование в Timestamp
        var startTime = Timestamp.FromDateTime(startDateTime.ToUniversalTime());
        var endTime = Timestamp.FromDateTime(endDateTime.ToUniversalTime());

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
