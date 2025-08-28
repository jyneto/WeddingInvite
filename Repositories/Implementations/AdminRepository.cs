using Microsoft.EntityFrameworkCore;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;
namespace WeddingInvite.Api.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly WeddingContext _context;
        public AdminRepository(WeddingContext context)
        {
            _context = context;
        }
        public async Task<Admin?> GetByUserNameAsync(string userName)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.UserName == userName);
            return admin;
        }
    }
}
