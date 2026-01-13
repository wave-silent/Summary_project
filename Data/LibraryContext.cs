using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using LibraryWPFApp.Models;

namespace LibraryWPFApp.Data
{
    /// <summary>
    /// Контекст базы данных для библиотечной системы.
    /// Обеспечивает взаимодействие между объектами C# и таблицами PostgreSQL.
    /// </summary>
    public class LibraryContext : DbContext
    {
        /// <summary>
        /// Инициализирует новый экземпляр контекста базы данных.
        /// Использует строку подключения "LibraryConnection" из App.config.
        /// </summary>
        public LibraryContext() : base("name=LibraryConnection")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            Configuration.UseDatabaseNullSemantics = true;
        }

        /// <summary>
        /// Набор данных для работы с авторами.
        /// </summary>
        public DbSet<Author> Authors { get; set; }

        /// <summary>
        /// Набор данных для работы с книгами.
        /// </summary>
        public DbSet<Book> Books { get; set; }

        /// <summary>
        /// Набор данных для работы с читателями.
        /// </summary>
        public DbSet<Reader> Readers { get; set; }

        /// <summary>
        /// Набор данных для работы с экземплярами книг.
        /// </summary>
        public DbSet<Copy> Copies { get; set; }

        /// <summary>
        /// Набор данных для работы с выдачами книг.
        /// </summary>
        public DbSet<Loan> Loans { get; set; }

        /// <summary>
        /// Набор данных для работы с блокировками читателей.
        /// </summary>
        public DbSet<Block> Blocks { get; set; }

        /// <summary>
        /// Настраивает связи между таблицами при создании модели.
        /// </summary>
        /// <param name="modelBuilder">Построитель модели Entity Framework.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Author>().ToTable("authors", "public");
            modelBuilder.Entity<Book>().ToTable("books", "public");
            modelBuilder.Entity<Reader>().ToTable("readers", "public");
            modelBuilder.Entity<Copy>().ToTable("bookcopies", "public");
            modelBuilder.Entity<Loan>().ToTable("loans", "public");
            modelBuilder.Entity<Block>().ToTable("blocks", "public");

            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books)
                .WithRequired(b => b.Author)
                .HasForeignKey(b => b.Author_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Copies)
                .WithRequired(c => c.Book)
                .HasForeignKey(c => c.Book_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reader>()
                .HasMany(r => r.Loans)
                .WithRequired(l => l.Reader)
                .HasForeignKey(l => l.User_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Copy>()
                .HasMany(c => c.Loans)
                .WithRequired(l => l.Copy)
                .HasForeignKey(l => l.Copy_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reader>()
                .HasMany(r => r.Blocks)
                .WithRequired(b => b.Reader)
                .HasForeignKey(b => b.User_id)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Тестирует подключение к базе данных.
        /// </summary>
        /// <returns>True если подключение успешно, иначе False.</returns>
        public bool TestConnection()
        {
            try
            {
                return Database.Exists();
            }
            catch
            {
                return false;
            }
        }
    }
}
