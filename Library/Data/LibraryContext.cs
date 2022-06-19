using Microsoft.EntityFrameworkCore;
using Library.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Library.Data
{
    public class LibraryContext : IdentityDbContext<ApplicationUser>
    {

        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        { }

        public DbSet<Author> Author { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<AuthorBook> AuthorBook { get; set; }
        public DbSet<StudentBook> StudentBook{ get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreBook> StoreBooks { get; set; }
        public DbSet<RatingStudentBook> RatingStudentBook { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Author>()
                        .HasMany(x => x.AuthorBooks)
                        .WithOne(x => x.Author)
                        .HasForeignKey(x => x.AuthorId);

            modelBuilder.Entity<Book>()
                        .HasMany(x => x.AuthorBooks)
                        .WithOne(x => x.Book)
                        .HasForeignKey(x => x.BookId);

            modelBuilder.Entity<Student>()
                        .HasMany(x => x.StudentBooks)
                        .WithOne(x => x.Student)
                        .HasForeignKey(x => x.StudentId);

            modelBuilder.Entity<Book>()
                        .HasMany(x => x.StudentBooks)
                        .WithOne(x => x.Book)
                        .HasForeignKey(x => x.BookId);

            modelBuilder.Entity<Store>()
                       .HasMany(x => x.StoreBooks)
                       .WithOne(x => x.Store)
                       .HasForeignKey(x => x.StoreId);

            modelBuilder.Entity<Book>()
                        .HasMany(x => x.StoreBooks)
                        .WithOne(x => x.Book)
                        .HasForeignKey(x => x.BookId);

            modelBuilder.Entity<Book>()
                        .HasMany(x => x.RatingStudentBooks)
                        .WithOne(x => x.Book)
                        .HasForeignKey(x => x.BookId);

            modelBuilder.Entity<Student>()
                        .HasMany(x => x.RatingStudentBooks)
                        .WithOne(x => x.Student)
                        .HasForeignKey(x => x.StudentId);

        }
    }
}
