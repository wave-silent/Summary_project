namespace LibraryWPFApp
{
    /// <summary>
    /// Класс для хранения информации о доступном экземпляре книги.
    /// Используется для отображения списка доступных книг при выдаче.
    /// </summary>
    public class AvailableCopyInfo
    {
        /// <summary>
        /// Идентификатор экземпляра книги.
        /// </summary>
        public int Copy_id { get; set; }

        /// <summary>
        /// Название книги.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Имя автора книги.
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// Год издания книги.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Отформатированная информация о книге для отображения.
        /// </summary>
        public string DisplayInfo
        {
            get
            {
                return Title + " (" + AuthorName + ", " + Year + ")";
            }
        }
    }
}
