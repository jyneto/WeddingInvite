using WeddingInvite.Api.DTOs.AuthDTO;

namespace WeddingInvite.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AdminResponseDTO?> LoginAsync(AdminLoginDTO adminLoginDTO);
        Task<string> HashPassword (string password);
        Task<bool> VerifyPassword(string password, string hash);
    }
}
