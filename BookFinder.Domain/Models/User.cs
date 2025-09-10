namespace BookFinder.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = String.Empty;
    public string PasswordHash { get; set; } = String.Empty;
}