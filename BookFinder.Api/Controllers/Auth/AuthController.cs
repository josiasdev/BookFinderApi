using BookFinder.Domain.DTOs.Auth;
using BookFinder.Domain.DTOs.User;
using BookFinder.Domain.Models;
using BookFinder.Infrastructure.Data;
using BookFinder.Infrastructure.Services.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BookFinder.Api.Controllers.Auth;


[ApiController]
[Route("/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(ApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Registra um novo usuário no sistema.
    /// </summary>
    /// <param name="registerDto">Objeto contendo o nome de usuário e a senha.</param>
    /// <returns>O usuário recém-criado.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisteredUserDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username.ToLower()))
        {
            return BadRequest("Username already taken");
        }

        var user = new User
        {
            Username = registerDto.Username.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userResponse = new RegisteredUserDto
        {
            Id = user.Id,
            Username = user.Username
        };

        return CreatedAtAction(nameof(Register), new { id = user.Id }, userResponse);
    }

    /// <summary>
    /// Autentica um usuário e retorna um token JWT.
    /// </summary>
    /// <param name="loginDto">Objeto contendo o nome de usuário e a senha.</param>
    /// <returns>Um objeto com o nome de usuário e o token de acesso.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserDto), 200)] 
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username.ToLower());

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid username or password");
        }

        var userDto = new UserDto
        {
            Username = user.Username,
            Token = _tokenService.GenerateToken(user)
        };

        return Ok(userDto);
    }
}