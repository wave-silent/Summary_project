using System.Windows.Controls;

namespace LibraryWPFApp
{
    /// <summary>
    /// Пользовательский элемент управления для отображения и управления списком авторов.
    /// Предоставляет функционал просмотра, поиска, добавления, редактирования и удаления авторов.
    /// </summary>
    public partial class AuthorsView : UserControl
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса AuthorsView.
        /// Устанавливает DataContext на экземпляр AuthorViewModel.
        /// </summary>
        public AuthorsView()
        {
            InitializeComponent();
            DataContext = new AuthorViewModel();
        }
    }
}
