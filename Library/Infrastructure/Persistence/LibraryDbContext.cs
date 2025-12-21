using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<Person> People => Set<Person>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<AccountRole> AccountRoles => Set<AccountRole>();

    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookCategory> BookCategories => Set<BookCategory>();
    public DbSet<AvailableBook> AvailableBooks => Set<AvailableBook>();

    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Borrow> Borrows => Set<Borrow>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
    public DbSet<Fine> Fines => Set<Fine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>()
            .HasIndex(a => a.Username)
            .IsUnique();

        modelBuilder.Entity<Person>()
            .HasOne(p => p.Account)
            .WithOne(a => a.Person)
            .HasForeignKey<Account>(a => a.PersonId);

        modelBuilder.Entity<AccountRole>()
            .HasKey(x => new { x.AccountId, x.RoleId });
        modelBuilder.Entity<AccountRole>()
            .HasOne(x => x.Account)
            .WithMany(a => a.AccountRoles)
            .HasForeignKey(x => x.AccountId);
        modelBuilder.Entity<AccountRole>()
            .HasOne(x => x.Role)
            .WithMany(r => r.AccountRoles)
            .HasForeignKey(x => x.RoleId);

        modelBuilder.Entity<BookCategory>()
            .HasKey(x => new { x.BookId, x.CategoryId });
        modelBuilder.Entity<BookCategory>()
            .HasOne(x => x.Book)
            .WithMany(b => b.BookCategories)
            .HasForeignKey(x => x.BookId);
        modelBuilder.Entity<BookCategory>()
            .HasOne(x => x.Category)
            .WithMany(c => c.BookCategories)
            .HasForeignKey(x => x.CategoryId);

        modelBuilder.Entity<AvailableBook>()
            .HasIndex(x => x.InventoryCode)
            .IsUnique();

        modelBuilder.Entity<Fine>()
            .Property(f => f.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<BookAuthor>()
            .HasKey(x => new { x.BookId, x.AuthorId });
        modelBuilder.Entity<BookAuthor>()
            .HasOne(x => x.Book)
            .WithMany(b => b.BookAuthors)
            .HasForeignKey(x => x.BookId);
        modelBuilder.Entity<BookAuthor>()
            .HasOne(x => x.Author)
            .WithMany(a => a.BookAuthors)
            .HasForeignKey(x => x.AuthorId);
    }
}


