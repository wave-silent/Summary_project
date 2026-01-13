using LibraryWPFApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

/// <summary>
/// Класс, представляющий читателя в библиотечной системе.
/// Соответствует таблице "readers" в базе данных.
/// </summary>
[Table("readers")]
public class Reader
{
    /// <summary>
    /// Уникальный идентификатор читателя.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("user_id")]
    public int User_id { get; set; }

    /// <summary>
    /// Фамилия читателя.
    /// </summary>
    [Column("last_name")]
    public string Fam { get; set; }

    /// <summary>
    /// Имя читателя.
    /// </summary>
    [Column("first_name")]
    public string Imya { get; set; }

    /// <summary>
    /// Отчество читателя.
    /// </summary>
    [Column("middle_name")]
    public string Otch { get; set; }

    /// <summary>
    /// Дата рождения читателя.
    /// </summary>
    [Column("birth_date")]
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Контактный телефон читателя.
    /// </summary>
    [Column("phone")]
    public string Phone { get; set; }

    /// <summary>
    /// Электронная почта читателя.
    /// </summary>
    [Column("email")]
    public string Email { get; set; }

    /// <summary>
    /// Адрес проживания читателя.
    /// </summary>
    [Column("address")]
    public string Address { get; set; }

    /// <summary>
    /// Флаг, указывающий, может ли читатель брать книги на прокат.
    /// </summary>
    [Column("get_rent")]
    public bool? GetRent { get; set; }

    /// <summary>
    /// Дата, к которой читатель должен вернуть все книги.
    /// </summary>
    [Column("must_return")]
    public DateTime? MustReturn { get; set; }

    /// <summary>
    /// Полное имя читателя (вычисляемое свойство).
    /// </summary>
    [NotMapped]
    public string FullName
    {
        get
        {
            return $"{Fam} {Imya} {Otch}".Trim();
        }
    }

    /// <summary>
    /// Навигационное свойство для выданных книг.
    /// Содержит список всех выданных книг (выдач) для данного читателя.
    /// </summary>
    public virtual ICollection<Loan> Loans { get; set; }

    /// <summary>
    /// Навигационное свойство для блокировок читателя.
    /// Содержит список всех блокировок, примененных к данному читателя.
    /// </summary>
    public virtual ICollection<Block> Blocks { get; set; }

    /// <summary>
    /// Конструктор по умолчанию.
    /// Инициализирует коллекции навигационных свойств.
    /// </summary>
    public Reader()
    {
        Loans = new List<Loan>();
        Blocks = new List<Block>();
    }
}