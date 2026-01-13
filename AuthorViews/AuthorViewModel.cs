using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using LibraryWPFApp.Data;
using System.Data.Entity;
using System.Windows;


namespace LibraryWPFApp
{
    /// <summary>
    /// ViewModel для управления авторами в библиотечной системе.
    /// Предоставляет функционал для загрузки, фильтрации, добавления, 
    /// редактирования и удаления авторов.
    /// </summary>
    public class AuthorViewModel : INotifyPropertyChanged
    {
        private readonly LibraryContext _context;
        private string _searchText;
        private Author _selectedAuthor;

        /// <summary>
        /// Событие, возникающее при изменении значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Список авторов.
        /// </summary>
        public List<Author> Authors { get; set; }

        /// <summary>
        /// Выбранный автор.
        /// </summary>
        public Author SelectedAuthor
        {
            get { return _selectedAuthor; }
            set
            {
                _selectedAuthor = value;
                OnPropertyChanged("SelectedAuthor");
            }
        }

        /// <summary>
        /// Текст для поиска авторов.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
                FilterAuthors();
            }
        }

        /// <summary>
        /// Сообщение о статусе операций.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Количество авторов в текущем списке.
        /// </summary>
        public int AuthorCount
        {
            get
            {
                if (Authors != null)
                    return Authors.Count;
                return 0;
            }
        }

        /// <summary>
        /// Команда для добавления нового автора.
        /// </summary>
        public ICommand AddAuthorCommand { get; }

        /// <summary>
        /// Команда для редактирования выбранного автора.
        /// </summary>
        public ICommand EditAuthorCommand { get; }

        /// <summary>
        /// Команда для удаления выбранного автора.
        /// </summary>
        public ICommand DeleteAuthorCommand { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса AuthorViewModel.
        /// </summary>
        public AuthorViewModel()
        {
            _context = new LibraryContext();
            Authors = new List<Author>();

            AddAuthorCommand = new RelayCommand(AddAuthor);
            EditAuthorCommand = new RelayCommand(EditAuthor, CanEditOrDelete);
            DeleteAuthorCommand = new RelayCommand(DeleteAuthor, CanEditOrDelete);

            LoadData();
        }

        /// <summary>
        /// Загружает данные об авторах из базы данных.
        /// </summary>
        private void LoadData()
        {
            try
            {
                Authors = _context.Authors
                    .Include(a => a.Books)
                    .OrderBy(a => a.Fam)
                    .ToList();

                StatusMessage = "Данные загружены успешно";
                OnPropertyChanged("Authors");
                OnPropertyChanged("AuthorCount");
            }
            catch (Exception ex)
            {
                StatusMessage = "Ошибка загрузки: " + ex.Message;
            }
        }

        /// <summary>
        /// Фильтрует список авторов по поисковому запросу.
        /// </summary>
        private void FilterAuthors()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Authors = _context.Authors.ToList();
            }
            else
            {
                Authors = _context.Authors
                    .Where(a => a.Fam.Contains(SearchText) ||
                                a.Imya.Contains(SearchText) ||
                                a.Otch.Contains(SearchText))
                    .ToList();
            }
            OnPropertyChanged("Authors");
            OnPropertyChanged("AuthorCount");
        }

        /// <summary>
        /// Открывает окно для добавления нового автора.
        /// </summary>
        private void AddAuthor()
        {
            try
            {
                var editViewModel = new AuthorEditViewModel(_context, null);
                var window = new AuthorEditWindow { DataContext = editViewModel };

                if (window.ShowDialog() == true)
                {
                    LoadData();
                    StatusMessage = "Автор успешно добавлен";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при открытии окна добавления: " + ex.Message + "\n\n" + ex.StackTrace,
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Открывает окно для редактирования выбранного автора.
        /// </summary>
        private void EditAuthor()
        {
            try
            {
                if (SelectedAuthor == null) return;

                var editViewModel = new AuthorEditViewModel(_context, SelectedAuthor);
                var window = new AuthorEditWindow { DataContext = editViewModel };

                if (window.ShowDialog() == true)
                {
                    LoadData();
                    StatusMessage = "Автор успешно обновлен";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при открытии окна редактирования: " + ex.Message + "\n\n" + ex.StackTrace,
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Проверяет, можно ли редактировать или удалить автора.
        /// </summary>
        /// <returns>True, если автор выбран, иначе False.</returns>
        private bool CanEditOrDelete()
        {
            return SelectedAuthor != null;
        }

        /// <summary>
        /// Удаляет выбранного автора после подтверждения.
        /// </summary>
        private void DeleteAuthor()
        {
            if (SelectedAuthor == null) return;

            if (SelectedAuthor.Books != null && SelectedAuthor.Books.Any())
            {
                MessageBox.Show("Нельзя удалить автора '" + SelectedAuthor.FullName + "', так как у него есть книги. Сначала удалите или переназначьте книги.",
                    "Ошибка удаления",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Удалить автора '" + SelectedAuthor.FullName + "'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Authors.Remove(SelectedAuthor);
                    _context.SaveChanges();
                    StatusMessage = "Автор удален успешно";
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