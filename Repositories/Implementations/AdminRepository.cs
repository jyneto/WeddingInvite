using Microsoft.EntityFrameworkCore;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;
namespace WeddingInvite.Api.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly WeddingDbContext _context;
        public AdminRepository(WeddingDbContext context)
        {
            _context = context;
        }
        public async Task<Admin?> GetByUserNameAsync(string userName)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.UserName == userName);
            return admin;
        }

        public async Task<bool> AddAdminAsync(Admin admin)
        {
            _context.Admins.Add(admin);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await _context.Admins.AnyAsync(a => a.UserName == userName);
        }
        public async Task<bool> AnyAsync()
        {
            return await _context.Admins.AnyAsync();
        }
    }
}
