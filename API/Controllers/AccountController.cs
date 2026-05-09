using API.Dtos;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : ApiController
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    public AccountController(
        AppDbContext appDbContext,
        ITokenService tokenService)
    {
        _context = appDbContext;
        _tokenService = tokenService;
    }
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        if (await UserExists(registerDto.Email))
        {
            return BadRequest("Email is already taken.");
        }
        var user = new AppUser
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key

        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var userDto = new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Token = _tokenService.CreateToken(user),
            Email = user.Email
        };
        return Ok(userDto);
    }
    private async Task<bool> UserExists(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email.ToLowerInvariant() == email.ToLowerInvariant());
    }
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Email.ToLowerInvariant() == loginDto.Email.ToLowerInvariant());
        if (user == null)
        {
            return Unauthorized("Invalid email.");
        }
        using var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDto.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return Unauthorized("Invalid password.");
            }
        }
        var userDto = new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Token = _tokenService.CreateToken(user),
            Email = user.Email,

        };

        return Ok(userDto);
    }
}