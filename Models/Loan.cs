using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryWPFApp.Models
{
    /// <summary>
    /// Класс, представляющий выдачу книги в библиотечной системе.
    /// Соответствует таблице "loans" в базе данных.
    /// </summary>
    [Table("loans")]
    public class Loan
    {
        /// <summary>
        /// Уникальный идентификатор выдачи.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("loan_id")]
        [Display(Name = "ID выдачи")]
        public int Loan_id { get; set; }

        /// <summary>
        /// Идентификатор читателя, которому выдана книга.
        /// </summary>
        [Required]
        [Column("user_id")]
        [Display(Name = "ID читателя")]
        public int User_id { get; set; }

        /// <summary>
        /// Идентификатор экземпляра книги, который был выдан.
        /// </summary>
        [Required]
        [Column("copy_id")]
        [Display(Name = "ID экземпляра")]
        public int Copy_id { get; set; }

        /// <summary>
        /// Дата выдачи книги читателю.
        /// </summary>
        [Required]
        [Column("bear_date")]
        [Display(Name = "Дата выдачи")]
        [DataType(DataType.Date)]
        public DateTime BearDate { get; set; }

        /// <summary>
        /// Планируемая дата возврата книги.
        /// </summary>
        [Required]
        [Column("due_date")]
        [Display(Name = "Дата возврата")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Фактическая дата возврата книги.
        /// </summary>
        [Column("return_date")]
        [Display(Name = "Фактический возврат")]
        [DataType(DataType.Date)]
        public DateTime? Return_date { get; set; }

        /// <summary>
        /// Флаг, указывающий, была ли книга возвращена.
        /// </summary>
        [Column("is_returned")]
        [Display(Name = "Возвращена")]
        public bool IsReturned { get; set; }

        /// <summary>
        /// Конструктор по умолчанию.
        /// Инициализирует свойства значениями по умолчанию.
        /// </summary>
        public Loan()
        {
            IsReturned = false;
        }

        /// <summary>
        /// Статус выдачи (вычисляемое свойство).
        /// Возвращает: "Возвращена", "Просрочена" или "На руках".
        /// </summary>
        [NotMapped]
        public string Status
        {
            get
            {
                if (IsReturned || Return_date != null)
                {
                    return "Возвращена";
                }
                else if (DateTime.Now > DueDate)
                {
                    return "Просрочена";
                }
                else
                {
                    return "На руках";
                }
            }
        }

        /// <summary>
        /// Флаг просрочки (вычисляемое свойство).
        /// Возвращает true, если книга не возвращена и срок возврата истек.
        /// </summary>
        [NotMapped]
        public bool IsOverdue
        {
            get
            {
                return !IsReturned && DateTime.Now > DueDate && Return_date == null;
            }
        }

        /// <summary>
        /// Навигационное свойство для читателя, которому выдана книга.
        /// </summary>
        [ForeignKey("User_id")]
        public virtual Reader Reader { get; set; }

        /// <summary>
        /// Навигационное свойство для экземпляра книги, который был выдан.
        /// </summary>
        [ForeignKey("Copy_id")]
        public virtual Copy Copy { get; set; }
    }
}