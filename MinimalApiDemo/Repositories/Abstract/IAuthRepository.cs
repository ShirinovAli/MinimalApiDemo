using MinimalApiDemo.DTOs;

namespace MinimalApiDemo.Repositories.Abstract
{
    public interface IAuthRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<UserDto> Register(RegistrationRequestDto registrationRequestDto);
    }
}
