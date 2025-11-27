using backend.DTOs;
using backend.Models;

namespace backend.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> Authenticate(string email, string password);
        Task<UserDto> Register(string name, string email, string password);
    }
}