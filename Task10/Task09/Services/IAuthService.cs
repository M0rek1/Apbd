using Task09.DTOs;

namespace Task09.Services;

public interface IAuthService
{
    void RegisterUser(RegisterUserDto request);
    (string accessToken, string refreshToken) LoginUser(LoginDto request);
}