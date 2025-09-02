using WeddingInvite.Api.DTOs.MenuItemDTO;

namespace WeddingInvite.Api.Services.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuItemGetDTO>> GetAllItemsAsync();
        Task<MenuItemGetDTO?> GetItemByIdAsync(int id);
        Task<int> AddItemAsync(MenuItemCreateDTO menuItemCreateDTO);
        Task<bool> UpdateItemAsync(MenuItemGetDTO menuItemRequestDTO);
        Task<bool> DeleteItemAsync(int id);

    }
}
