using System.Windows.Controls;

namespace LibraryWPFApp
{
    /// <summary>
    /// Пользовательский элемент управления для отображения и управления списком книг.
    /// Предоставляет функционал просмотра, поиска, добавления, редактирования и удаления книг.
    /// </summary>
    public partial class BooksView : UserControl
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса BooksView.
        /// Устанавливает DataContext на экземпляр BookViewModel.
        /// </summary>
        public BooksView()
        {
            InitializeComponent();
            DataContext = new BookViewModel();
        }
    }
}
