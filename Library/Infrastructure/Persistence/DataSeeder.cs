using Library.Application.Repositories;
using Library.Application.Services;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Library.Infrastructure.Persistence;

public class DataSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(IServiceProvider serviceProvider, ILogger<DataSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // Ensure database exists and apply migrations
        // MigrateAsync() will create the database if it doesn't exist
        _logger.LogInformation("Ensuring database exists and applying migrations...");
        await context.Database.MigrateAsync();
        _logger.LogInformation("Database migrations applied successfully");

        // Seed Roles
        if (!await unitOfWork.Roles.AnyAsync(r => r.Name == "Admin"))
        {
            _logger.LogInformation("Seeding roles...");
            var adminRole = new Role { Id = Guid.NewGuid(), Name = "Admin" };
            var memberRole = new Role { Id = Guid.NewGuid(), Name = "Member" };
            var librarianRole = new Role { Id = Guid.NewGuid(), Name = "Librarian" };

            unitOfWork.Roles.Add(adminRole);
            unitOfWork.Roles.Add(memberRole);
            unitOfWork.Roles.Add(librarianRole);
            await unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Roles seeded successfully");
        }

        // Seed Default Admin Account
        if (!await unitOfWork.Accounts.UsernameExistsAsync("admin"))
        {
            _logger.LogInformation("Seeding default admin account...");
            
            var adminPerson = new Person
            {
                Id = Guid.NewGuid(),
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@library.local"
            };

            var adminRole = await unitOfWork.Roles.GetByNameAsync("Admin");
            if (adminRole == null)
            {
                throw new InvalidOperationException("Admin role not found");
            }

            // Security: Use strong default password - MUST be changed in production!
            var defaultPassword = "Admin@123!ChangeMe"; // This should be changed immediately
            var adminAccount = new Account
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                PasswordHash = passwordHasher.HashPassword(defaultPassword),
                IsActive = true,
                PersonId = adminPerson.Id
            };

            adminAccount.AccountRoles.Add(new AccountRole
            {
                AccountId = adminAccount.Id,
                RoleId = adminRole.Id
            });

            unitOfWork.People.Add(adminPerson);
            unitOfWork.Accounts.Add(adminAccount);
            await unitOfWork.SaveChangesAsync();
            
            _logger.LogWarning("Default admin account created. Username: admin, Password: Admin@123!ChangeMe - CHANGE THIS IMMEDIATELY!");
        }

        // Seed Categories
        if (await unitOfWork.Categories.CountAsync() == 0)
        {
            _logger.LogInformation("Seeding categories...");
            var categories = new[]
            {
                new Category { Id = Guid.NewGuid(), Name = "Fiction" },
                new Category { Id = Guid.NewGuid(), Name = "Non-Fiction" },
                new Category { Id = Guid.NewGuid(), Name = "Science" },
                new Category { Id = Guid.NewGuid(), Name = "History" },
                new Category { Id = Guid.NewGuid(), Name = "Biography" },
                new Category { Id = Guid.NewGuid(), Name = "Technology" },
                new Category { Id = Guid.NewGuid(), Name = "Literature" },
                new Category { Id = Guid.NewGuid(), Name = "Education" }
            };

            foreach (var category in categories)
            {
                unitOfWork.Categories.Add(category);
            }
            await unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Categories seeded successfully");
        }

        // Seed Publishers
        if (await unitOfWork.Publishers.CountAsync() == 0)
        {
            _logger.LogInformation("Seeding publishers...");
            var publishers = new[]
            {
                new Publisher { Id = Guid.NewGuid(), Name = "Penguin Random House" },
                new Publisher { Id = Guid.NewGuid(), Name = "HarperCollins" },
                new Publisher { Id = Guid.NewGuid(), Name = "Simon & Schuster" },
                new Publisher { Id = Guid.NewGuid(), Name = "Macmillan Publishers" },
                new Publisher { Id = Guid.NewGuid(), Name = "Hachette Book Group" }
            };

            foreach (var publisher in publishers)
            {
                unitOfWork.Publishers.Add(publisher);
            }
            await unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Publishers seeded successfully");
        }

        // Seed Authors
        if (await unitOfWork.Authors.CountAsync() == 0)
        {
            _logger.LogInformation("Seeding authors...");
            var authors = new[]
            {
                new Author { Id = Guid.NewGuid(), FirstName = "George", LastName = "Orwell", Biography = "English novelist and essayist" },
                new Author { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Austen", Biography = "English novelist" },
                new Author { Id = Guid.NewGuid(), FirstName = "Stephen", LastName = "King", Biography = "American author of horror and suspense novels" },
                new Author { Id = Guid.NewGuid(), FirstName = "J.K.", LastName = "Rowling", Biography = "British author and philanthropist" },
                new Author { Id = Guid.NewGuid(), FirstName = "Isaac", LastName = "Asimov", Biography = "American writer and professor of biochemistry" }
            };

            foreach (var author in authors)
            {
                unitOfWork.Authors.Add(author);
            }
            await unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Authors seeded successfully");
        }

        _logger.LogInformation("Database seeding completed");
    }
}

