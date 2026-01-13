using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using LibraryWPFApp.Data;
using System.Windows;

namespace LibraryWPFApp
{
    /// <summary>
    /// ViewModel для окна добавления и редактирования читателей.
    /// Обеспечивает ввод и валидацию данных читателя, сохранение в базу данных.
    /// </summary>
    public class ReaderEditViewModel : INotifyPropertyChanged
    {
        private readonly LibraryContext _context;
        private readonly Reader _reader;
        private string _fam;
        private string _imya;
        private string _otch;
        private DateTime? _birthDate;
        private string _phone;
        private string _email;
        private string _address;
        private bool? _getRent;
        private DateTime? _mustReturn;

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
                if (_reader == null)
                    return "Добавить читателя";
                return "Редактировать читателя";
            }
        }

        /// <summary>
        /// Фамилия читателя.
        /// </summary>
        public string Fam
        {
            get { return _fam; }
            set
            {
                _fam = value;
                OnPropertyChanged("Fam");
            }
        }

        /// <summary>
        /// Имя читателя.
        /// </summary>
        public string Imya
        {
            get { return _imya; }
            set
            {
                _imya = value;
                OnPropertyChanged("Imya");
            }
        }

        /// <summary>
        /// Отчество читателя.
        /// </summary>
        public string Otch
        {
            get { return _otch; }
            set
            {
                _otch = value;
                OnPropertyChanged("Otch");
            }
        }

        /// <summary>
        /// Дата рождения читателя.
        /// </summary>
        public DateTime? BirthDate
        {
            get { return _birthDate; }
            set
            {
                _birthDate = value;
                OnPropertyChanged("BirthDate");
            }
        }

        /// <summary>
        /// Телефон читателя.
        /// </summary>
        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                OnPropertyChanged("Phone");
            }
        }

        /// <summary>
        /// Email читателя.
        /// </summary>
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }

        /// <summary>
        /// Адрес читателя.
        /// </summary>
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged("Address");
            }
        }

        /// <summary>
        /// Флаг наличия книги у читателя.
        /// </summary>
        public bool? GetRent
        {
            get { return _getRent; }
            set
            {
                _getRent = value;
                OnPropertyChanged("GetRent");
            }
        }

        /// <summary>
        /// Дата, до которой читатель должен вернуть книгу.
        /// </summary>
        public DateTime? MustReturn
        {
            get { return _mustReturn; }
            set
            {
                _mustReturn = value;
                OnPropertyChanged("MustReturn");
            }
        }

        /// <summary>
        /// Команда для сохранения читателя.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Команда для отмены редактирования.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса ReaderEditViewModel.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        /// <param name="reader">Читатель для редактирования или null для создания нового.</param>
        public ReaderEditViewModel(LibraryContext context, Reader reader)
        {
            _context = context;
            _reader = reader;

            if (reader != null)
            {
                Fam = reader.Fam;
                Imya = reader.Imya;
                Otch = reader.Otch;
                BirthDate = reader.BirthDate;
                Phone = reader.Phone;
                Email = reader.Email;
                Address = reader.Address;
                GetRent = reader.GetRent;
                MustReturn = reader.MustReturn;
            }

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        /// <summary>
        /// Определяет, можно ли сохранить данные читателя.
        /// </summary>
        /// <returns>True если указаны фамилия и имя, иначе False.</returns>
        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Fam) && !string.IsNullOrWhiteSpace(Imya);
        }

        /// <summary>
        /// Сохраняет данные читателя в базу данных.
        /// </summary>
        private void Save()
        {
            if (!CanSave()) return;

            try
            {
                if (_reader == null)
                {
                    var newReader = new Reader
                    {
                        Fam = Fam,
                        Imya = Imya,
                        Otch = Otch,
                        BirthDate = BirthDate,
                        Phone = Phone,
                        Email = Email,
                        Address = Address,
                        GetRent = GetRent != null ? GetRent.Value : false,
                        MustReturn = MustReturn
                    };
                    _context.Readers.Add(newReader);
                }
                else
                {
                    _reader.Fam = Fam;
                    _reader.Imya = Imya;
                    _reader.Otch = Otch;
                    _reader.BirthDate = BirthDate;
                    _reader.Phone = Phone;
                    _reader.Email = Email;
                    _reader.Address = Address;
                    _reader.GetRent = GetRent;
                    _reader.MustReturn = MustReturn;
                }

                _context.SaveChanges();
                CloseWindow(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Отменяет редактирование читателя.
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
