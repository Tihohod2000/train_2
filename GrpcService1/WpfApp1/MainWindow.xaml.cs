using System;
using System.Windows;
using Grpc.Net.Client;
using GrpcWagonService; // Пространство имен, которое будет сгенерировано на основе .proto
using Google.Protobuf.WellKnownTypes;
using System.Collections.ObjectModel;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private readonly WagonService.WagonServiceClient _client;

        public MainWindow()
        {
            InitializeComponent();
            var channel = GrpcChannel.ForAddress("http://localhost:5000");
            _client = new WagonService.WagonServiceClient(channel);
        }

        private async void OnGetWagonsClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем введенные данные
                string startDateText = StartDateTextBox.Text;
                string endDateText = EndDateTextBox.Text;
                
                if (string.IsNullOrEmpty(startDateText) || string.IsNullOrEmpty(endDateText))
                {
                    MessageBox.Show("Пожалуйста введите дату и время в указанном формате.");
                    return;
                }

                // Преобразуем строки в DateTime
                DateTime startDateTime = DateTime.ParseExact(startDateText, "yyyy-MM-dd HH:mm:ss.ffffff", null);
                DateTime endDateTime = DateTime.ParseExact(endDateText, "yyyy-MM-dd HH:mm:ss.ffffff", null);

                // Конвертируем в gRPC формат
                var startTime = Timestamp.FromDateTime(startDateTime.ToUniversalTime());
                var endTime = Timestamp.FromDateTime(endDateTime.ToUniversalTime());

                var request = new WagonRequest
                {
                    StartTime = startTime,
                    EndTime = endTime
                };

                // Запрос к серверу
                var response = await _client.GetWagonsAsync(request);

                // Отображаем результаты в DataGrid
                var wagonsList = new ObservableCollection<WagonInfo>();

                foreach (var wagon in response.Wagons)
                {
                    wagonsList.Add(new WagonInfo
                    {
                        InventoryNumber = wagon.InventoryNumber.ToString(),
                        ArrivalTime = wagon.ArrivalTime != null 
                            ? wagon.ArrivalTime.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss.ffffff") 
                            : "не указано", // Если ArrivalTime пустое, выводим "не указано"
                        DepartureTime = wagon.DepartureTime != null 
                            ? wagon.DepartureTime.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss.ffffff") 
                            : "не указано" // Если DepartureTime пустое, выводим "не указано"
                    });
                    Console.WriteLine(wagon.InventoryNumber.ToString());
                }

                WagonsDataGrid.ItemsSource = wagonsList;
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid date format. Please use 'yyyy-MM-dd HH:mm:ss'.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void StartDateTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            StartDatePlaceholder.Visibility = Visibility.Collapsed;
        }

        private void StartDateTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(StartDateTextBox.Text))
            {
                StartDatePlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void EndDateTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            EndDatePlaceholder.Visibility = Visibility.Collapsed;
        }

        private void EndDateTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EndDateTextBox.Text))
            {
                EndDatePlaceholder.Visibility = Visibility.Visible;
            }
        }
    }

    // Класс для отображения данных о вагоне
    public class WagonInfo
    {
        public string InventoryNumber { get; set; }
        public string ArrivalTime { get; set; }
        public string DepartureTime { get; set; }
    }
}
