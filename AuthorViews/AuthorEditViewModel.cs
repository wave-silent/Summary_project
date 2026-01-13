using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LibraryWPFApp.Data;

namespace LibraryWPFApp
{
    /// <summary>
    /// ViewModel для окна добавления и редактирования авторов.
    /// Обеспечивает валидацию данных и сохранение в базу данных.
    /// </summary>
    public class AuthorEditViewModel : INotifyPropertyChanged
    {
        private readonly LibraryContext _context;
        private readonly Author _author;
        private string _fam;
        private string _imya;
        private string _otch;

        /// <summary>
        /// Сообщение об ошибке для фамилии.
        /// </summary>
        public string FamError { get; private set; }

        /// <summary>
        /// Сообщение об ошибке для имени.
        /// </summary>
        public string ImyaError { get; private set; }

        /// <summary>
        /// Сообщение об ошибке для отчества.
        /// </summary>
        public string OtchError { get; private set; }

        /// <summary>
        /// Определяет, есть ли ошибки валидации.
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return !string.IsNullOrEmpty(FamError) ||
                       !string.IsNullOrEmpty(ImyaError) ||
                       !string.IsNullOrEmpty(OtchError);
            }
        }

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
                if (_author == null)
                    return "Добавление нового автора";
                return "Редактирование автора";
            }
        }

        /// <summary>
        /// Фамилия автора.
        /// </summary>
        public string Fam
        {
            get { return _fam; }
            set
            {
                _fam = value;
                ValidateFam();
                OnPropertyChanged("Fam");
                OnPropertyChanged("FamError");
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Имя автора.
        /// </summary>
        public string Imya
        {
            get { return _imya; }
            set
            {
                _imya = value;
                ValidateImya();
                OnPropertyChanged("Imya");
                OnPropertyChanged("ImyaError");
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Отчество автора.
        /// </summary>
        public string Otch
        {
            get { return _otch; }
            set
            {
                _otch = value;
                ValidateOtch();
                OnPropertyChanged("Otch");
                OnPropertyChanged("OtchError");
            }
        }

        /// <summary>
        /// Команда для сохранения автора.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Команда для отмены редактирования.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса AuthorEditViewModel.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        /// <param name="author">Автор для редактирования или null для создания нового.</param>
        public AuthorEditViewModel(LibraryContext context, Author author)
        {
            try
            {
                _context = context;
                _author = author;

                SaveCommand = new RelayCommand(Save, CanSave);
                CancelCommand = new RelayCommand(Cancel);

                if (author != null)
                {
                    _fam = author.Fam ?? "";
                    _imya = author.Imya ?? "";
                    _otch = author.Otch ?? "";

                    ValidateFam();
                    ValidateImya();
                    ValidateOtch();

                    OnPropertyChanged("Fam");
                    OnPropertyChanged("Imya");
                    OnPropertyChanged("Otch");
                    OnPropertyChanged("FamError");
                    OnPropertyChanged("ImyaError");
                    OnPropertyChanged("OtchError");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка в конструкторе: " + ex.Message,
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Валидирует фамилию автора.
        /// </summary>
        private void ValidateFam()
        {
            if (string.IsNullOrWhiteSpace(Fam))
            {
                FamError = "Фамилия обязательна для заполнения";
            }
            else if (Fam.Length < 2 || Fam.Length > 50)
            {
                FamError = "Фамилия должна быть от 2 до 50 символов";
            }
            else if (!char.IsUpper(Fam[0]))
            {
                FamError = "Фамилия должна начинаться с заглавной буквы";
            }
            else
            {
                FamError = null;
            }
        }

        /// <summary>
        /// Валидирует имя автора.
        /// </summary>
        private void ValidateImya()
        {
            if (string.IsNullOrWhiteSpace(Imya))
            {
                ImyaError = "Имя обязательно для заполнения";
            }
            else if (Imya.Length < 2 || Imya.Length > 30)
            {
                ImyaError = "Имя должно быть от 2 до 30 символов";
            }
            else if (!char.IsUpper(Imya[0]))
            {
                ImyaError = "Имя должно начинаться с заглавной буквы";
            }
            else
            {
                ImyaError = null;
            }
        }

        /// <summary>
        /// Валидирует отчество автора.
        /// </summary>
        private void ValidateOtch()
        {
            if (!string.IsNullOrWhiteSpace(Otch))
            {
                if (Otch.Length > 30)
                {
                    OtchError = "Отчество не должно превышать 30 символов";
                }
                else if (!char.IsUpper(Otch[0]))
                {
                    OtchError = "Отчество должно начинаться с заглавной буквы";
                }
                else
                {
                    OtchError = null;
                }
            }
            else
            {
                OtchError = null;
            }
        }

        /// <summary>
        /// Определяет, можно ли сохранить данные.
        /// </summary>
        /// <returns>True если нет ошибок валидации, иначе False.</returns>
        private bool CanSave()
        {
            return !HasErrors;
        }

        /// <summary>
        /// Сохраняет данные автора в базу данных.
        /// </summary>
        private void Save()
        {
            if (!CanSave()) return;

            try
            {
                if (_author == null)
                {
                    var newAuthor = new Author
                    {
                        Fam = Fam.Trim(),
                        Imya = Imya.Trim(),
                        Otch = Otch != null ? Otch.Trim() : null
                    };
                    _context.Authors.Add(newAuthor);
                }
                else
                {
                    _author.Fam = Fam.Trim();
                    _author.Imya = Imya.Trim();
                    _author.Otch = Otch != null ? Otch.Trim() : null;
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
        /// Отменяет редактирование.
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