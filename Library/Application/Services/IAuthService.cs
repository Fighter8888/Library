using Library.Application.DTOs.Auth;

namespace Library.Application.Services;

public interface IAuthService
{
    Task<AuthResponse?> SignupAsync(SignupRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}

