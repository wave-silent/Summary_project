using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryWPFApp.Models
{
    /// <summary>
    /// Класс, представляющий блокировку читателя в библиотечной системе.
    /// Соответствует таблице "blocks" в базе данных.
    /// </summary>
    [Table("blocks")]
    public class Block
    {
        /// <summary>
        /// Уникальный идентификатор блокировки.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("block_id")]
        [Display(Name = "ID блокировки")]
        public int Block_id { get; set; }

        /// <summary>
        /// Идентификатор читателя, к которому относится блокировка.
        /// </summary>
        [Required(ErrorMessage = "Читатель обязателен")]
        [Column("user_id")]
        [Display(Name = "ID читателя")]
        public int User_id { get; set; }

        /// <summary>
        /// Флаг, указывающий заблокирован ли читатель.
        /// </summary>
        [Column("is_blocked")]
        [Display(Name = "Заблокирован")]
        public bool? IsBlocked
        {
            get
            {
                return _isBlocked;
            }
            set
            {
                _isBlocked = value;
            }
        }

        private bool? _isBlocked;

        /// <summary>
        /// Причина блокировки читателя.
        /// </summary>
        [Column("block_reason")]
        [Display(Name = "Причина блокировки")]
        [StringLength(500, ErrorMessage = "Причина не должна превышать 500 символов")]
        public string BlockReason
        {
            get
            {
                return _blockReason;
            }
            set
            {
                _blockReason = value;
            }
        }

        private string _blockReason;

        /// <summary>
        /// Оплаченная сумма для разблокировки читателя.
        /// </summary>
        [Column("paid_amount")]
        [Display(Name = "Оплаченная сумма")]
        [Range(0, int.MaxValue, ErrorMessage = "Сумма не может быть отрицательной")]
        public int? PaidAmount
        {
            get
            {
                return _paidAmount;
            }
            set
            {
                _paidAmount = value;
            }
        }

        private int? _paidAmount;

        /// <summary>
        /// Навигационное свойство для читателя, к которому относится блокировка.
        /// </summary>
        [ForeignKey("User_id")]
        public virtual Reader Reader
        {
            get
            {
                return _reader;
            }
            set
            {
                _reader = value;
            }
        }

        private Reader _reader;
    }
}