using Library.Application.DTOs.Auth;
using Library.Application.Repositories;
using Library.Domain.Entities;

namespace Library.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse?> SignupAsync(SignupRequest request)
    {
        // Check if username already exists
        if (await _unitOfWork.Accounts.UsernameExistsAsync(request.Username))
        {
            return null; // Username already exists
        }

        // Create person
        var person = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        // Create account
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true,
            PersonId = person.Id
        };

        // Assign Member role by default
        var memberRole = await _unitOfWork.Roles.GetByNameAsync("Member");
        if (memberRole == null)
        {
            memberRole = new Role { Id = Guid.NewGuid(), Name = "Member" };
            _unitOfWork.Roles.Add(memberRole);
        }

        account.AccountRoles.Add(new AccountRole
        {
            AccountId = account.Id,
            RoleId = memberRole.Id
        });

        _unitOfWork.People.Add(person);
        _unitOfWork.Accounts.Add(account);
        await _unitOfWork.SaveChangesAsync();

        var roles = new List<string> { "Member" };
        var token = _jwtService.GenerateToken(account.Id, account.Username, roles);

        return new AuthResponse
        {
            Token = token,
            Username = account.Username,
            Roles = roles,
            AccountId = account.Id
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var account = await _unitOfWork.Accounts.GetByUsernameWithRolesAsync(request.Username);

        if (account == null || !account.IsActive)
        {
            return null; // Account not found or inactive
        }

        if (!_passwordHasher.VerifyPassword(request.Password, account.PasswordHash))
        {
            return null; // Invalid password
        }

        var roles = account.AccountRoles.Select(ar => ar.Role.Name).ToList();
        var token = _jwtService.GenerateToken(account.Id, account.Username, roles);

        return new AuthResponse
        {
            Token = token,
            Username = account.Username,
            Roles = roles,
            AccountId = account.Id
        };
    }
}

