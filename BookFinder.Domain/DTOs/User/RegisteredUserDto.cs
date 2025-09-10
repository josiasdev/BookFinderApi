namespace BookFinder.Domain.DTOs.User;

public class RegisteredUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
}