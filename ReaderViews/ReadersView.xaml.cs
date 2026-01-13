using System.Windows.Controls;

namespace LibraryWPFApp
{
    /// <summary>
    /// Пользовательский элемент управления для отображения и управления списком читателей.
    /// Предоставляет функционал просмотра, поиска, добавления, редактирования и удаления читателей.
    /// </summary>
    public partial class ReadersView : UserControl
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса ReadersView.
        /// Устанавливает DataContext на экземпляр ReaderViewModel.
        /// </summary>
        public ReadersView()
        {
            InitializeComponent();
            DataContext = new ReaderViewModel();
        }
    }
}
