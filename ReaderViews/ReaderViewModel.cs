using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using LibraryWPFApp.Data;
using System.Windows;

namespace LibraryWPFApp
{
    /// <summary>
    /// ViewModel для управления читателями в библиотечной системе.
    /// Предоставляет функционал для загрузки, фильтрации, добавления, 
    /// редактирования и удаления читателей.
    /// </summary>
    public class ReaderViewModel : INotifyPropertyChanged
    {
        private readonly LibraryContext _context;
        private string _searchText;
        private Reader _selectedReader;

        /// <summary>
        /// Событие, возникающее при изменении значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Список читателей.
        /// </summary>
        public List<Reader> Readers { get; set; }

        /// <summary>
        /// Выбранный читатель.
        /// </summary>
        public Reader SelectedReader
        {
            get { return _selectedReader; }
            set
            {
                _selectedReader = value;
                OnPropertyChanged("SelectedReader");
            }
        }

        /// <summary>
        /// Текст для поиска читателей.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
                FilterReaders();
            }
        }

        /// <summary>
        /// Сообщение о статусе операций.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Количество читателей в текущем списке.
        /// </summary>
        public int ReaderCount
        {
            get
            {
                if (Readers != null)
                    return Readers.Count;
                return 0;
            }
        }

        /// <summary>
        /// Команда для добавления нового читателя.
        /// </summary>
        public ICommand AddReaderCommand { get; }

        /// <summary>
        /// Команда для редактирования выбранного читателя.
        /// </summary>
        public ICommand EditReaderCommand { get; }

        /// <summary>
        /// Команда для удаления выбранного читателя.
        /// </summary>
        public ICommand DeleteReaderCommand { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса ReaderViewModel.
        /// </summary>
        public ReaderViewModel()
        {
            _context = new LibraryContext();
            Readers = new List<Reader>();

            AddReaderCommand = new RelayCommand(AddReader);
            EditReaderCommand = new RelayCommand(EditReader, CanEditOrDelete);
            DeleteReaderCommand = new RelayCommand(DeleteReader, CanEditOrDelete);

            LoadData();
        }

        /// <summary>
        /// Загружает данные о читателях из базы данных.
        /// </summary>
        private void LoadData()
        {
            try
            {
                Readers = _context.Readers.ToList();

                OnPropertyChanged("Readers");
                OnPropertyChanged("ReaderCount");
                StatusMessage = "Данные загружены успешно";
            }
            catch (Exception ex)
            {
                StatusMessage = "Ошибка загрузки: " + ex.Message;
            }
        }

        /// <summary>
        /// Фильтрует список читателей по поисковому запросу.
        /// </summary>
        private void FilterReaders()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Readers = _context.Readers.ToList();
            }
            else
            {
                Readers = _context.Readers
                    .Where(r => r.Fam.Contains(SearchText) ||
                                r.Imya.Contains(SearchText) ||
                                r.Otch.Contains(SearchText) ||
                                r.Phone.Contains(SearchText) ||
                                r.Email.Contains(SearchText))
                    .ToList();
            }
            OnPropertyChanged("Readers");
            OnPropertyChanged("ReaderCount");
        }

        /// <summary>
        /// Открывает окно для добавления нового читателя.
        /// </summary>
        private void AddReader()
        {
            var editViewModel = new ReaderEditViewModel(_context, null);
            var window = new ReaderEditWindow { DataContext = editViewModel };

            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        /// <summary>
        /// Открывает окно для редактирования выбранного читателя.
        /// </summary>
        private void EditReader()
        {
            if (SelectedReader == null) return;

            var editViewModel = new ReaderEditViewModel(_context, SelectedReader);
            var window = new ReaderEditWindow { DataContext = editViewModel };

            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        /// <summary>
        /// Проверяет, можно ли редактировать или удалить читателя.
        /// </summary>
        /// <returns>True, если читатель выбран, иначе False.</returns>
        private bool CanEditOrDelete()
        {
            return SelectedReader != null;
        }

        /// <summary>
        /// Удаляет выбранного читателя после подтверждения.
        /// </summary>
        private void DeleteReader()
        {
            if (SelectedReader == null) return;

            var result = MessageBox.Show(
                "Удалить читателя '" + SelectedReader.FullName + "'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Readers.Remove(SelectedReader);
                    _context.SaveChanges();
                    StatusMessage = "Читатель удален успешно";
                    LoadData();
                }
                catch (Exception ex)
                {
                    StatusMessage = "Ошибка удаления: " + ex.Message;
                }
            }
        }

        /// <summary>
        /// Вызывает событие PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Имя измененного свойства.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}