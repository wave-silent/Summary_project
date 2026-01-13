using LibraryWPFApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Класс, представляющий книгу в библиотечной системе.
/// Соответствует таблице "books" в базе данных.
/// </summary>
[Table("books")]
public class Book
{
    /// <summary>
    /// Уникальный идентификатор книги.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("book_id")]
    public int Book_id { get; set; }

    /// <summary>
    /// Название книги.
    /// </summary>
    [Required]
    [Column("title")]
    public string Title { get; set; }

    /// <summary>
    /// Год издания книги.
    /// </summary>
    [Column("year")]
    public int? Year { get; set; }

    /// <summary>
    /// Идентификатор автора книги.
    /// </summary>
    [Column("author_id")]
    public int Author_id { get; set; }

    /// <summary>
    /// Язык книги.
    /// </summary>
    [Column("language")]
    public string Language { get; set; }

    /// <summary>
    /// Количество страниц в книге.
    /// </summary>
    [Column("pages")]
    public int? Pages { get; set; }

    /// <summary>
    /// Навигационное свойство для автора книги.
    /// </summary>
    [ForeignKey("Author_id")]
    public virtual Author Author { get; set; }

    /// <summary>
    /// Навигационное свойство для экземпляров книги.
    /// </summary>
    public virtual ICollection<Copy> Copies { get; set; }

    public Book()
    {
        Copies = new List<Copy>();
    }

    /// <summary>
    /// Имя автора книги (вычисляемое свойство).
    /// </summary>
    [NotMapped]
    public string AuthorName
    {
        get
        {
            if (Author != null && Author.FullName != null)
                return Author.FullName;
            return "Автор не указан";
        }
    }
}