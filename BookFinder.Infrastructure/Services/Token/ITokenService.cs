using BookFinder.Domain.Models;

namespace BookFinder.Infrastructure.Services.Token;

public interface ITokenService
{
    string GenerateToken(User user);
}