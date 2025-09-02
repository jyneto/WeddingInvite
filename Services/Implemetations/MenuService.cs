using WeddingInvite.Api.DTOs.MenuItemDTO;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Services.Implemetations
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<int> AddItemAsync(MenuItemCreateDTO menuItemRequestDTO)
        {
            var newMenuItem = new MenuItem
            {
                Name = menuItemRequestDTO.Name,
                Description = menuItemRequestDTO.Description,
                Price = menuItemRequestDTO.Price
            };
            return await _menuRepository.AddItemAsync(newMenuItem);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var menu = await _menuRepository.GetItemByIdAsync(id);
            if (menu == null)
            {
                return false;
            }
            return await _menuRepository.DeleteItemAsync(menu.Id);
        }
        public async Task<List<MenuItemGetDTO>> GetAllItemsAsync()
        {
            var menus = await _menuRepository.GetAllItemAsync();
            var menuDTOs = menus.Select(m => new MenuItemGetDTO
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price
            }).ToList();
            return menuDTOs;
        }

        public async Task<MenuItemGetDTO?> GetItemByIdAsync(int id)
        {
            var menuItem = await _menuRepository.GetItemByIdAsync(id);
            if (menuItem == null)
            {
                return null;
            }
            var menuItemDTO = new MenuItemGetDTO
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price
            };
            return menuItemDTO;
        }

        public Task<bool> UpdateItemAsync(MenuItemGetDTO menuItemRequestDTO)
        {
            var existingMenuItem = _menuRepository.GetItemByIdAsync(menuItemRequestDTO.Id);
            if (existingMenuItem == null)
            {
                return Task.FromResult(false);
            }
            var updatedMenuItem = new Models.MenuItem
            {
                Id = menuItemRequestDTO.Id,
                Name = menuItemRequestDTO.Name,
                Description = menuItemRequestDTO.Description,
                Price = menuItemRequestDTO.Price
            };
            return _menuRepository.UpdateItemAsync(updatedMenuItem);
        }
    }
}
