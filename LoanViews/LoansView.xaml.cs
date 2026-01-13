using System.Windows.Controls;

namespace LibraryWPFApp
{
    /// <summary>
    /// Пользовательский элемент управления для управления выдачами книг.
    /// Предоставляет функционал просмотра активных выдач, выдачи книг и отметки о возврате.
    /// </summary>
    public partial class LoansView : UserControl
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса LoansView.
        /// Устанавливает DataContext на экземпляр LoanViewModel.
        /// </summary>
        public LoansView()
        {
            InitializeComponent();
            DataContext = new LoanViewModel();
        }
    }
}