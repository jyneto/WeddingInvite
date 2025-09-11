using Microsoft.EntityFrameworkCore;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;

namespace WeddingInvite.Api.Repositories.Implementations
{
    public class MenuRepository : IMenuRepository
    {
        private readonly WeddingDbContext _context;
        public MenuRepository(WeddingDbContext context)
        {
            _context = context;
        }
        public async Task<int> AddItemAsync(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            return menuItem.Id;
        }

        public async Task<bool> DeleteItemAsync(int menuItemId)
        {
            var rowsAffected = await _context.MenuItems
                .Where(m => m.Id == menuItemId)
                .ExecuteDeleteAsync();
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<List<MenuItem>> GetAllItemAsync()
        {
            var items = await _context.MenuItems.ToListAsync();
            return items;
        }
        public async Task<MenuItem?> GetItemByIdAsync(int id)
        {
            var item = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id);
            return item;
        }
        public async Task<bool> UpdateItemAsync(MenuItem menuItem)
        {
            //_context.MenuItems.Update(menuItem);
            //var result = _context.SaveChangesAsync();
            //if (result.Result > 0)
            //{
            //    return Task.FromResult(true);
            //}
            //return Task.FromResult(false);

            var existingItem = await _context.MenuItems.FindAsync(menuItem.Id);
            if (existingItem == null) return false;

            existingItem.Name = menuItem.Name;
            existingItem.Description = menuItem.Description;
            existingItem.Price = menuItem.Price;

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }


    }
}
