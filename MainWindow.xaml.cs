using System;
using System.Windows;
using System.Windows.Media;

namespace LibraryWPFApp
{
    /// <summary>
    /// Основное окно приложения библиотечной системы.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Конструктор основного окна.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события загрузки окна.
        /// Выполняет инициализацию данных при запуске приложения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatus("Программа готова к работе", Colors.Green);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка загрузки: {ex.InnerException.Message ?? ex.Message}", Colors.Red);
            }
        }

        /// <summary>
        /// Обновляет статус в строке состояния.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        /// <param name="color">Цвет текста сообщения.</param>
        private void UpdateStatus(string message, Color color)
        {
            StatusText.Text = message;
            StatusText.Foreground = new SolidColorBrush(color);
        }

        /// <summary>
        /// Обновляет счетчик записей в интерфейсе.
        /// </summary>
        private void UpdateRecordCount()
        {
            RecordCountText.Text = "Система загружена";
        }

        /// <summary>
        /// Обработчик нажатия кнопки обновления всех данных.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void RefreshAll_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Обновление данных...", Colors.Blue);

            UpdateStatus("Данные могут быть обновлены в соответствующих вкладках", Colors.Green);
        }

        /// <summary>
        /// Обработчик нажатия кнопки выхода из приложения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "О программе".
        /// Отображает информацию о приложении.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Библиотечная система v1.0\n" +
                          "PostgreSQL + Entity Framework\n" +
                          "CRUD операции для всех таблиц",
                          "О программе",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
