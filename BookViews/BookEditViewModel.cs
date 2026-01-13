using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LibraryWPFApp.Data;

namespace LibraryWPFApp
{
    /// <summary>
    /// ViewModel для окна добавления и редактирования книг.
    /// Обеспечивает работу с данными книги и сохранение в базу данных.
    /// </summary>
    public class BookEditViewModel : INotifyPropertyChanged
    {
        private readonly LibraryContext _context;
        private readonly Book _book;
        private string _title;
        private int? _year;
        private string _language;
        private int? _pages;
        private int _selectedAuthorId;

        /// <summary>
        /// Событие, возникающее при изменении значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Заголовок окна редактирования.
        /// </summary>
        public string WindowTitle
        {
            get
            {
                if (_book == null)
                    return "Добавить книгу";
                return "Редактировать книгу";
            }
        }

        /// <summary>
        /// Название книги.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        /// <summary>
        /// Год издания книги.
        /// </summary>
        public int? Year
        {
            get { return _year; }
            set
            {
                _year = value;
                OnPropertyChanged("Year");
            }
        }

        /// <summary>
        /// Язык книги.
        /// </summary>
        public string Language
        {
            get { return _language; }
            set
            {
                _language = value;
                OnPropertyChanged("Language");
            }
        }

        /// <summary>
        /// Количество страниц в книге.
        /// </summary>
        public int? Pages
        {
            get { return _pages; }
            set
            {
                _pages = value;
                OnPropertyChanged("Pages");
            }
        }

        /// <summary>
        /// Идентификатор выбранного автора.
        /// </summary>
        public int SelectedAuthorId
        {
            get { return _selectedAuthorId; }
            set
            {
                _selectedAuthorId = value;
                OnPropertyChanged("SelectedAuthorId");
            }
        }

        /// <summary>
        /// Список доступных авторов.
        /// </summary>
        public List<Author> Authors { get; set; }

        /// <summary>
        /// Команда для сохранения книги.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Команда для отмены редактирования.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса BookEditViewModel.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        /// <param name="book">Книга для редактирования или null для создания новой.</param>
        public BookEditViewModel(LibraryContext context, Book book)
        {
            _context = context;
            _book = book;

            Authors = context.Authors.ToList();

            if (book != null)
            {
                Title = book.Title;
                Year = book.Year;
                Language = book.Language;
                Pages = book.Pages;
                SelectedAuthorId = book.Author_id;
            }
            else if (Authors.Any())
            {
                SelectedAuthorId = Authors.First().Author_id;
            }

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        /// <summary>
        /// Определяет, можно ли сохранить книгу.
        /// </summary>
        /// <returns>True если есть название и выбран автор, иначе False.</returns>
        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Title) && SelectedAuthorId > 0;
        }

        /// <summary>
        /// Сохраняет книгу в базу данных.
        /// </summary>
        private void Save()
        {
            if (!CanSave()) return;

            try
            {
                if (_book == null)
                {
                    var newBook = new Book
                    {
                        Title = Title,
                        Year = Year,
                        Language = Language,
                        Pages = Pages,
                        Author_id = SelectedAuthorId
                    };

                    _context.Books.Add(newBook);
                }
                else
                {
                    _book.Title = Title;
                    _book.Year = Year;
                    _book.Language = Language;
                    _book.Pages = Pages;
                    _book.Author_id = SelectedAuthorId;
                }

                _context.SaveChanges();
                CloseWindow(true);
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                string errorMessage = "Ошибка выполнения SQL команды:\n" + ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "\n\nВнутренняя ошибка:\n" + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        errorMessage += "\n\nДетали:\n" + ex.InnerException.InnerException.Message;
                    }
                }

                MessageBox.Show(errorMessage, "SQL Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                string errorMessage = "Ошибка обновления БД:\n" + ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "\n\nВнутренняя ошибка:\n" + ex.InnerException.Message;
                }

                MessageBox.Show(errorMessage, "Ошибка БД",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message + "\n\nStackTrace:\n" + ex.StackTrace,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Отменяет редактирование книги.
        /// </summary>
        private void Cancel()
        {
            CloseWindow(false);
        }

        /// <summary>
        /// Закрывает окно редактирования.
        /// </summary>
        /// <param name="result">Результат диалога: true - сохранено, false - отмена.</param>
        private void CloseWindow(bool result)
        {
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            if (window != null)
            {
                window.DialogResult = result;
                window.Close();
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
