using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Task09.Context;
using Task09.DTOs;
using Task09.Helpers;
using Task09.Models;

namespace Task09.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public void RegisterUser(RegisterUserDto request)
    {
        var hashedPasswordAndSalt = SecurityHelpers.GetHashedPasswordAndSalt(request.Password);
        var user = new User()
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Password = hashedPasswordAndSalt.Item1,
            Salt = hashedPasswordAndSalt.Item2,
            RefreshToken = SecurityHelpers.GenerateRefreshToken(),
            RefreshTokenExp = DateTime.Now.AddDays(1)
        };
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public (string accessToken, string refreshToken) LoginUser(LoginDto request)
    {
        var user = _context.Users.FirstOrDefault(e =>  e.Email == request.Email);
        if (user == null) throw new Exception("User not found");

        string passwordHashFromDb = user.Password;
        string curHashedPassword = SecurityHelpers.GetHashedPasswordWithSalt(request.Password, user.Salt);
        if (passwordHashFromDb != curHashedPassword) throw new Exception("Incorrect password!");

        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.Id)),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, "User")
        };

        SymmetricSecurityKey sskey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
        SigningCredentials credentials = new SigningCredentials(sskey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "https://localhost:5001",
            audience: "https://localhost:5001",
            claims: userClaims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: credentials
        );

        user.RefreshToken = SecurityHelpers.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);
        _context.SaveChanges();

        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        string refreshToken = user.RefreshToken;

        return (accessToken, refreshToken);
    }

}