using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using LibraryWPFApp.Data;
using LibraryWPFApp.Models;
using System.Data.Entity;
using System.Windows;

namespace LibraryWPFApp
{
    /// <summary>
    /// ViewModel для окна выдачи книг читателям.
    /// Обеспечивает выбор читателя и книги, установку сроков возврата.
    /// </summary>
    public class LoanEditViewModel : INotifyPropertyChanged
    {
        private readonly LibraryContext _context;
        private int _selectedReaderId;
        private int _selectedCopyId;
        private DateTime _bearDate;
        private DateTime _dueDate;
        private bool _isTwoWeeks;
        private bool _isOneMonth;
        private string _note;
        private string _selectedBookInfo;

        /// <summary>
        /// Событие, возникающее при изменении значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Список доступных читателей.
        /// </summary>
        public List<Reader> Readers { get; set; }

        /// <summary>
        /// Список доступных экземпляров книг.
        /// </summary>
        public List<AvailableCopyInfo> AvailableBooks { get; set; }

        /// <summary>
        /// Идентификатор выбранного читателя.
        /// </summary>
        public int SelectedReaderId
        {
            get { return _selectedReaderId; }
            set
            {
                _selectedReaderId = value;
                OnPropertyChanged("SelectedReaderId");
                OnPropertyChanged("CanSave");
            }
        }

        /// <summary>
        /// Идентификатор выбранного экземпляра книги.
        /// </summary>
        public int SelectedCopyId
        {
            get { return _selectedCopyId; }
            set
            {
                _selectedCopyId = value;
                OnPropertyChanged("SelectedCopyId");
                OnPropertyChanged("CanSave");
                UpdateSelectedBookInfo();
            }
        }

        /// <summary>
        /// Дата выдачи книги.
        /// </summary>
        public DateTime BearDate
        {
            get { return _bearDate; }
            set
            {
                _bearDate = value;
                OnPropertyChanged("BearDate");
                UpdateDueDate();
            }
        }

        /// <summary>
        /// Дата возврата книги.
        /// </summary>
        public DateTime DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                OnPropertyChanged("DueDate");
            }
        }

        /// <summary>
        /// Флаг выбора срока на 2 недели.
        /// </summary>
        public bool IsTwoWeeks
        {
            get { return _isTwoWeeks; }
            set
            {
                _isTwoWeeks = value;
                OnPropertyChanged("IsTwoWeeks");
                if (value) UpdateDueDate();
            }
        }

        /// <summary>
        /// Флаг выбора срока на 1 месяц.
        /// </summary>
        public bool IsOneMonth
        {
            get { return _isOneMonth; }
            set
            {
                _isOneMonth = value;
                OnPropertyChanged("IsOneMonth");
                if (value) UpdateDueDate();
            }
        }

        /// <summary>
        /// Примечание к выдаче.
        /// </summary>
        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                OnPropertyChanged("Note");
            }
        }

        /// <summary>
        /// Информация о выбранной книге.
        /// </summary>
        public string SelectedBookInfo
        {
            get { return _selectedBookInfo; }
            set
            {
                _selectedBookInfo = value;
                OnPropertyChanged("SelectedBookInfo");
            }
        }

        /// <summary>
        /// Команда для сохранения выдачи книги.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Команда для отмены выдачи.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Определяет, можно ли сохранить выдачу.
        /// </summary>
        public bool CanSave
        {
            get { return SelectedReaderId > 0 && SelectedCopyId > 0; }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса LoanEditViewModel.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public LoanEditViewModel(LibraryContext context)
        {
            _context = context;

            // Инициализация значений по умолчанию
            _isTwoWeeks = true;
            _isOneMonth = false;
            _note = "";
            _selectedBookInfo = "Выберите книгу...";

            BearDate = DateTime.Today;
            DueDate = DateTime.Today.AddDays(14);

            LoadReaders();
            LoadAvailableBooks();

            SaveCommand = new RelayCommand(Save, CanSaveExecute);
            CancelCommand = new RelayCommand(Cancel);
        }

        /// <summary>
        /// Загружает список доступных читателей.
        /// </summary>
        private void LoadReaders()
        {
            try
            {
                var allReaders = _context.Readers.ToList();
                var readerIds = allReaders.Select(r => r.User_id).ToList();
                var blocks = _context.Blocks
                    .Where(b => readerIds.Contains(b.User_id) && b.IsBlocked == true)
                    .ToList();

                var blockedReaderIds = blocks.Select(b => b.User_id).ToHashSet();

                Readers = allReaders
                    .Where(r => !blockedReaderIds.Contains(r.User_id))
                    .OrderBy(r => r.Fam)
                    .ToList();

                if (!Readers.Any())
                {
                    MessageBox.Show("Нет доступных читателей для выдачи книг",
                                   "Внимание",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Readers = new List<Reader>();
                MessageBox.Show("Ошибка загрузки читателей: " + ex.Message + "\n\n" +
                               "Детали: " + ex.InnerException?.Message,
                               "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Загружает список доступных экземпляров книг.
        /// </summary>
        private void LoadAvailableBooks()
        {
            try
            {
                var availableCopies = _context.Copies
                    .Include(c => c.Book.Author)
                    .Where(c => c.IsAvailable)
                    .ToList()
                    .Select(c => new AvailableCopyInfo
                    {
                        Copy_id = c.Copy_id,
                        Title = c.Book != null ? c.Book.Title : "Без названия",
                        AuthorName = c.Book != null && c.Book.Author != null
                            ? c.Book.Author.Fam + " " + c.Book.Author.Imya + " " + c.Book.Author.Otch
                            : "Автор неизвестен",
                        Year = c.Book != null && c.Book.Year.HasValue ? c.Book.Year.Value : 0
                    })
                    .OrderBy(c => c.Title)
                    .ToList();

                AvailableBooks = availableCopies;

                if (!AvailableBooks.Any())
                {
                    MessageBox.Show("Нет доступных экземпляров книг для выдачи",
                                   "Внимание",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                AvailableBooks = new List<AvailableCopyInfo>();
                MessageBox.Show("Ошибка загрузки книг: " + ex.Message + "\n\nДетали: " + ex.InnerException?.Message,
                    "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обновляет дату возврата книги.
        /// </summary>
        private void UpdateDueDate()
        {
            if (IsTwoWeeks)
            {
                DueDate = BearDate.AddDays(14);
            }
            else if (IsOneMonth)
            {
                DueDate = BearDate.AddDays(30);
            }
        }

        /// <summary>
        /// Обновляет информацию о выбранной книге.
        /// </summary>
        private void UpdateSelectedBookInfo()
        {
            if (SelectedCopyId > 0)
            {
                var selectedCopy = AvailableBooks.FirstOrDefault(c => c.Copy_id == SelectedCopyId);
                if (selectedCopy != null)
                {
                    SelectedBookInfo = "Название: " + selectedCopy.Title + "\n" +
                                      "Автор: " + selectedCopy.AuthorName + "\n" +
                                      "Год: " + selectedCopy.Year;
                }
            }
            else
            {
                SelectedBookInfo = "Выберите книгу...";
            }
        }

        /// <summary>
        /// Определяет, можно ли выполнить команду сохранения.
        /// </summary>
        /// <returns>True если команда может выполняться, иначе False.</returns>
        private bool CanSaveExecute()
        {
            return CanSave;
        }

        /// <summary>
        /// Сохраняет выдачу книги в базу данных.
        /// </summary>
        private void Save()
        {
            if (!CanSave) return;

            try
            {
                var reader = _context.Readers
                    .Include(r => r.Blocks)
                    .FirstOrDefault(r => r.User_id == SelectedReaderId);

                if (reader == null)
                {
                    MessageBox.Show("Читатель не найден", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var readerBlocks = _context.Blocks
                    .Where(b => b.User_id == SelectedReaderId && b.IsBlocked == true)
                    .ToList();

                if (readerBlocks.Any())
                {
                    var block = readerBlocks.First();
                    MessageBox.Show("Читатель заблокирован!\nПричина: " + block.BlockReason,
                                   "Читатель заблокирован",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (reader.GetRent == true)
                {
                    MessageBox.Show("Читатель уже имеет книгу на руках", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var copy = _context.Copies
                    .Include(c => c.Loans)
                    .FirstOrDefault(c => c.Copy_id == SelectedCopyId);

                if (copy == null)
                {
                    MessageBox.Show("Экземпляр книги не найден", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!copy.IsAvailable)
                {
                    MessageBox.Show("Этот экземпляр книги недоступен для выдачи", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var hasActiveLoan = copy.Loans.Any(l => l.IsReturned == false);
                if (hasActiveLoan)
                {
                    MessageBox.Show("Этот экземпляр книги уже выдан другому читателю", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newLoan = new Loan
                {
                    User_id = SelectedReaderId,
                    Copy_id = SelectedCopyId,
                    BearDate = BearDate,
                    DueDate = DueDate,
                    Return_date = null,
                    IsReturned = false
                };

                _context.Loans.Add(newLoan);
                copy.IsAvailable = false;
                reader.GetRent = true;
                reader.MustReturn = DueDate;

                _context.SaveChanges();

                MessageBox.Show("Книга успешно выдана!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                CloseWindow(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Отменяет выдачу книги.
        /// </summary>
        private void Cancel()
        {
            CloseWindow(false);
        }

        /// <summary>
        /// Закрывает окно выдачи.
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