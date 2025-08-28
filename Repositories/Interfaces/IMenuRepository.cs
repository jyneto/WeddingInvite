using WeddingInvite.Api.Models;

namespace WeddingInvite.Api.Repositories.Interfaces
{
    public interface IMenuRepository
    {
        Task<List<MenuItem>> GetAllItemAsync();
        Task<MenuItem?> GetItemByIdAsync(int id);
        Task<int> AddItemAsync(MenuItem menuItem);
        Task<bool> UpdateItemAsync(MenuItem menuItem);
        Task<bool> DeleteItemAsync(int menuItemId);
    }
}
