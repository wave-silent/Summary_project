using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Класс, представляющий автора в библиотечной системе.
/// Соответствует таблице "authors" в базе данных.
/// </summary>
[Table("authors")]
public class Author
{
    /// <summary>
    /// Уникальный идентификатор автора.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("author_id")]
    public int Author_id { get; set; }

    /// <summary>
    /// Фамилия автора.
    /// </summary>
    [Column("last_name")]
    public string Fam { get; set; }

    /// <summary>
    /// Имя автора.
    /// </summary>
    [Column("first_name")]
    public string Imya { get; set; }

    /// <summary>
    /// Отчество автора.
    /// </summary>
    [Column("middle_name")]
    public string Otch { get; set; }

    /// <summary>
    /// Полное имя автора (фамилия, имя, отчество).
    /// </summary>
    [NotMapped]
    public string FullName
    {
        get
        {
            return Fam + " " + Imya + " " + Otch;
        }
    }

    /// <summary>
    /// Навигационное свойство для книг, написанных автором.
    /// </summary>
    public virtual ICollection<Book> Books
    {
        get
        {
            return _books;
        }
        set
        {
            _books = value;
        }
    }

    private ICollection<Book> _books = new List<Book>();

    /// <summary>
    /// Количество книг, написанных автором.
    /// </summary>
    [NotMapped]
    public int BookCount
    {
        get
        {
            if (Books != null)
                return Books.Count;
            return 0;
        }
    }
}
