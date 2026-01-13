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
    /// ViewModel для управления выдачами книг.
    /// Предоставляет функционал для просмотра активных выдач, выдачи книг читателям 
    /// и отметки о возврате книг.
    /// </summary>
    public class LoanViewModel : INotifyPropertyChanged
    {
        private readonly LibraryContext _context;
        private Loan _selectedLoan;

        /// <summary>
        /// Событие, возникающее при изменении значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Список активных выдач книг.
        /// </summary>
        public List<Loan> ActiveLoans { get; set; }

        /// <summary>
        /// Выбранная выдача книги.
        /// </summary>
        public Loan SelectedLoan
        {
            get { return _selectedLoan; }
            set
            {
                _selectedLoan = value;
                OnPropertyChanged("SelectedLoan");
                OnPropertyChanged("CanReturn");
            }
        }

        /// <summary>
        /// Определяет, можно ли отметить возврат выбранной книги.
        /// </summary>
        public bool CanReturn
        {
            get
            {
                if (SelectedLoan != null && SelectedLoan.Return_date == null)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Количество активных выдач книг.
        /// </summary>
        public int ActiveLoanCount
        {
            get
            {
                if (ActiveLoans != null)
                    return ActiveLoans.Count(l => l.Return_date == null);
                return 0;
            }
        }

        /// <summary>
        /// Количество просроченных выдач книг.
        /// </summary>
        public int OverdueCount
        {
            get
            {
                if (ActiveLoans != null)
                    return ActiveLoans.Count(l => l.IsOverdue);
                return 0;
            }
        }

        /// <summary>
        /// Сообщение о статусе операций.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Команда для создания новой выдачи книги.
        /// </summary>
        public ICommand AddLoanCommand { get; }

        /// <summary>
        /// Команда для отметки о возврате книги.
        /// </summary>
        public ICommand ReturnBookCommand { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса LoanViewModel.
        /// </summary>
        public LoanViewModel()
        {
            _context = new LibraryContext();
            ActiveLoans = new List<Loan>();

            AddLoanCommand = new RelayCommand(AddLoan);
            ReturnBookCommand = new RelayCommand(ReturnBook, CanReturnExecute);

            LoadData();
        }

        /// <summary>
        /// Загружает данные о активных выдачах книг из базы данных.
        /// </summary>
        private void LoadData()
        {
            try
            {
                ActiveLoans = _context.Loans
                    .Include(l => l.Reader)
                    .Include(l => l.Copy.Book.Author)
                    .Where(l => l.Return_date == null)
                    .OrderByDescending(l => l.BearDate)
                    .ToList();

                OnPropertyChanged("ActiveLoans");
                OnPropertyChanged("ActiveLoanCount");
                OnPropertyChanged("OverdueCount");
                StatusMessage = "Данные загружены успешно";
            }
            catch (Exception ex)
            {
                StatusMessage = "Ошибка загрузки: " + ex.Message;
            }
        }

        /// <summary>
        /// Открывает окно для создания новой выдачи книги.
        /// </summary>
        private void AddLoan()
        {
            var editViewModel = new LoanEditViewModel(_context);
            var window = new LoanEditWindow { DataContext = editViewModel };

            if (window.ShowDialog() == true)
            {
                LoadData();
                UpdateReaderStatus(editViewModel.SelectedReaderId, true);
            }
        }

        /// <summary>
        /// Отмечает возврат выбранной книги.
        /// </summary>
        private void ReturnBook()
        {
            if (SelectedLoan == null || SelectedLoan.Return_date != null) return;

            var result = MessageBox.Show(
                "Отметить возврат книги '" + SelectedLoan.Copy?.Book?.Title + "'?\n" +
                "Читатель: " + SelectedLoan.Reader?.FullName,
                "Подтверждение возврата",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedLoan.Return_date = DateTime.Today;

                    if (SelectedLoan.Copy != null)
                    {
                        SelectedLoan.Copy.IsAvailable = true;
                    }

                    _context.SaveChanges();
                    UpdateReaderStatus(SelectedLoan.User_id, false);

                    StatusMessage = "Книга возвращена успешно";
                    LoadData();
                }
                catch (Exception ex)
                {
                    StatusMessage = "Ошибка: " + ex.Message;
                }
            }
        }

        /// <summary>
        /// Определяет, можно ли выполнить команду возврата книги.
        /// </summary>
        /// <returns>True если команда может выполняться, иначе False.</returns>
        private bool CanReturnExecute()
        {
            return CanReturn;
        }

        /// <summary>
        /// Обновляет статус читателя после выдачи или возврата книги.
        /// </summary>
        /// <param name="readerId">Идентификатор читателя.</param>
        /// <param name="hasBook">Флаг наличия книги у читателя.</param>
        private void UpdateReaderStatus(int readerId, bool hasBook)
        {
            try
            {
                var reader = _context.Readers.Find(readerId);
                if (reader != null)
                {
                    reader.GetRent = hasBook;
                    if (!hasBook)
                        reader.MustReturn = null;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Отладочная информация сохраняется в логах
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