using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task09.DTOs;
using Task09.Services;

namespace Task09.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost]
    [Route("register")]
    [AllowAnonymous]
    public IActionResult RegisterUser([FromBody] RegisterUserDto request)
    {
        _auth.RegisterUser(request);
        return Ok();
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginDto request)
    {
        var (accessToken, refreshToken) = _auth.LoginUser(request);
        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }    

}