using WeddingInvite.Api.Models;

namespace WeddingInvite.Api.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByUserNameAsync(string userName);
    }
}
