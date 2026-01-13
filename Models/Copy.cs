using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryWPFApp.Models
{
    /// <summary>
    /// Класс, представляющий экземпляр книги в библиотечной системе.
    /// Соответствует таблице "bookcopies" в базе данных.
    /// </summary>
    [Table("bookcopies")]
    public class Copy
    {
        /// <summary>
        /// Уникальный идентификатор экземпляра книги.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("copy_id")]
        public int Copy_id { get; set; }

        /// <summary>
        /// Идентификатор книги, к которой относится данный экземпляр.
        /// </summary>
        [Required]
        [Column("book_id")]
        public int Book_id { get; set; }

        /// <summary>
        /// Флаг, указывающий, доступен ли экземпляр для выдачи.
        /// </summary>
        [Column("is_available")]
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Навигационное свойство для книги, к которой относится данный экземпляр.
        /// </summary>
        [ForeignKey("Book_id")]
        public virtual Book Book { get; set; }

        /// <summary>
        /// Навигационное свойство для выданных экземпляров.
        /// Содержит список всех выдач данного экземпляра книги.
        /// </summary>
        public virtual ICollection<Loan> Loans { get; set; }

        /// <summary>
        /// Конструктор по умолчанию.
        /// Инициализирует свойства значениями по умолчанию.
        /// </summary>
        public Copy()
        {
            IsAvailable = true;
            Loans = new List<Loan>();
        }
    }
}