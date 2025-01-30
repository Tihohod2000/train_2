using System;
using System.Windows;
using GrpcWagonService;
using Google.Protobuf.WellKnownTypes;
using System.Collections.ObjectModel;
using WpfApp1.Data;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private readonly IClientService _clientService;

        // Конструктор для получения зависимости через DI
        public MainWindow(IClientService clientService)
        {
            InitializeComponent();
            _clientService = clientService;
        }

        private async void OnGetWagonsClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем введенные данные
                string startDateText = StartDateTextBox.Text;
                string endDateText = EndDateTextBox.Text;

                // Проверка на пустые строки
                if (string.IsNullOrEmpty(startDateText) || string.IsNullOrEmpty(endDateText))
                {
                    MessageBox.Show("Please enter both start and end dates.");
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
                var response = await _clientService.GetWagonsAsync(request);

                // Отображаем результаты в DataGrid
                var wagonsList = new ObservableCollection<WagonInfo>();

                foreach (var wagon in response.Wagons)
                {
                    wagonsList.Add(new WagonInfo
                    {
                        InventoryNumber = wagon.InventoryNumber.ToString(),
                        ArrivalTime = wagon.ArrivalTime != null 
                            ? wagon.ArrivalTime.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff") 
                            : "не указано",
                        DepartureTime = wagon.DepartureTime != null 
                            ? wagon.DepartureTime.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff") 
                            : "не указано"
                    });
                    // Console.WriteLine(wagon.InventoryNumber.ToString());
                }

                WagonsDataGrid.ItemsSource = wagonsList;
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid date format. Please use 'yyyy-MM-dd HH:mm:ss.fff'.");
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
