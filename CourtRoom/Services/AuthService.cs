using Microsoft.AspNetCore.Identity;

namespace CourtRoom.Services;

public interface IAuthService
{
    public string HashPassword(string username, string password);

    public bool VerifyHashedPassword(string username, string plainPassword, string hashedPassword);
}

public class AuthService : IAuthService
{
    private readonly PasswordHasher<string> _hasher = new();

    public string HashPassword(string username, string plainPassword)
    {
        return _hasher.HashPassword(username, plainPassword);
    }

    public bool VerifyHashedPassword(string username, string plainPassword, string hashedPassword)
    {
        return _hasher.VerifyHashedPassword(username, hashedPassword, plainPassword) ==
               PasswordVerificationResult.Success;
    }
}