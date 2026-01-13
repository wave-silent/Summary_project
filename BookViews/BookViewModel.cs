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
    /// ViewModel для управления книгами в библиотечной системе.
    /// Предоставляет функционал для загрузки, фильтрации, добавления, 
    /// редактирования и удаления книг.
    /// </summary>
    public class BookViewModel : INotifyPropertyChanged
    {
        private readonly LibraryContext _context;
        private string _searchText;
        private Book _selectedBook;

        /// <summary>
        /// Событие, возникающее при изменении значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Список книг.
        /// </summary>
        public List<Book> Books { get; set; }

        /// <summary>
        /// Список авторов (используется для привязки).
        /// </summary>
        public List<Author> Authors { get; set; }

        /// <summary>
        /// Выбранная книга.
        /// </summary>
        public Book SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                _selectedBook = value;
                OnPropertyChanged("SelectedBook");
            }
        }

        /// <summary>
        /// Текст для поиска книг.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
                FilterBooks();
            }
        }

        /// <summary>
        /// Сообщение о статусе операций.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Количество книг в текущем списке.
        /// </summary>
        public int BookCount
        {
            get
            {
                if (Books != null)
                    return Books.Count;
                return 0;
            }
        }

        /// <summary>
        /// Команда для добавления новой книги.
        /// </summary>
        public ICommand AddBookCommand { get; }

        /// <summary>
        /// Команда для редактирования выбранной книги.
        /// </summary>
        public ICommand EditBookCommand { get; }

        /// <summary>
        /// Команда для удаления выбранной книги.
        /// </summary>
        public ICommand DeleteBookCommand { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса BookViewModel.
        /// </summary>
        public BookViewModel()
        {
            _context = new LibraryContext();
            Books = new List<Book>();
            Authors = new List<Author>();

            AddBookCommand = new RelayCommand(AddBook);
            EditBookCommand = new RelayCommand(EditBook, CanEditOrDelete);
            DeleteBookCommand = new RelayCommand(DeleteBook, CanEditOrDelete);

            LoadData();
        }

        /// <summary>
        /// Загружает данные о книгах и авторах из базы данных.
        /// </summary>
        private void LoadData()
        {
            try
            {
                Books = _context.Books
                    .Include(b => b.Author)
                    .OrderBy(b => b.Title)
                    .ToList();

                StatusMessage = "Данные загружены успешно";
                OnPropertyChanged("Books");
                OnPropertyChanged("BookCount");
            }
            catch (Exception ex)
            {
                StatusMessage = "Ошибка загрузки: " + ex.Message;
            }
        }

        /// <summary>
        /// Фильтрует список книг по поисковому запросу.
        /// </summary>
        private void FilterBooks()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Books = _context.Books.Include("Author").ToList();
            }
            else
            {
                Books = _context.Books
                    .Include("Author")
                    .Where(b => b.Title.Contains(SearchText) ||
                                (b.Author != null &&
                                 (b.Author.Fam.Contains(SearchText) ||
                                  b.Author.Imya.Contains(SearchText))))
                    .ToList();
            }
            OnPropertyChanged("Books");
            OnPropertyChanged("BookCount");
        }

        /// <summary>
        /// Открывает окно для добавления новой книги.
        /// </summary>
        private void AddBook()
        {
            var editViewModel = new BookEditViewModel(_context, null);
            var window = new BookEditWindow { DataContext = editViewModel };

            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        /// <summary>
        /// Открывает окно для редактирования выбранной книги.
        /// </summary>
        private void EditBook()
        {
            if (SelectedBook == null) return;

            var editViewModel = new BookEditViewModel(_context, SelectedBook);
            var window = new BookEditWindow { DataContext = editViewModel };

            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        /// <summary>
        /// Проверяет, можно ли редактировать или удалить книгу.
        /// </summary>
        /// <returns>True, если книга выбрана, иначе False.</returns>
        private bool CanEditOrDelete()
        {
            return SelectedBook != null;
        }

        /// <summary>
        /// Удаляет выбранную книгу после подтверждения.
        /// </summary>
        private void DeleteBook()
        {
            if (SelectedBook == null) return;

            var result = MessageBox.Show(
                "Удалить книгу '" + SelectedBook.Title + "'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Books.Remove(SelectedBook);
                    _context.SaveChanges();
                    StatusMessage = "Книга удалена успешно";
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