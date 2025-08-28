using Microsoft.EntityFrameworkCore;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;

namespace WeddingInvite.Api.Repositories.Implementations
{
    public class GuestRepository : IGuestRepository
    {
        private readonly WeddingContext _context;
        public GuestRepository(WeddingContext context)
        {
            _context = context;
        }

        public async Task <int> AddGuestAsync(Guest guest)
        {
           _context.Guests.Add(guest);
              await _context.SaveChangesAsync();
            return guest.Id;
        }

        public async Task<bool> DeleteGuestAsync(int guestId)
        {
            var rowsAffected = await _context.Guests
                .Where(g => g.Id == guestId)
                .ExecuteDeleteAsync();
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<List<Guest>> GetAllGuestAsync()
        {
          return await _context.Guests.ToListAsync();
           
        }

        public async Task<Guest?> GetByIdAsync(int guestId)
        {
            var guest = await _context.Guests
                .FirstOrDefaultAsync(g => g.Id == guestId);
            return guest;
        }

        public async Task<bool> UpdateGuestAsync(Guest guest)
        {
            _context.Guests.Update(guest);
            var result = await _context.SaveChangesAsync();
            if(result > 0)
            {
                return true;
            }
            return false;
        }
    }
}
