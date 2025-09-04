using WeddingInvite.Api.Models;

namespace WeddingInvite.Api.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByUserNameAsync(string userName);
        Task<bool> AddAdminAsync(Admin admin);
        Task<bool> UserNameExistsAsync(string userName);
        Task<bool> AnyAsync();
    }
}
