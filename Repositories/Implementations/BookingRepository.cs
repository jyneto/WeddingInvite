using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;
namespace WeddingInvite.Api.Repositories.Implementations
{
    public class BookingRepository : IBookingRespository
    {
        private readonly WeddingDbContext _context;
        public BookingRepository(WeddingDbContext context)
        {
            _context = context;
        }

        public async Task<bool> BookingOverlapAsync(int tableId, DateTime start, DateTime end, int? excludeBookingId = null)
        {
           return await _context.Bookings.AnyAsync(b =>
           b.FK_TableId == tableId &&
           (excludeBookingId == null || b.Id != excludeBookingId.Value) &&
           b.StartTime < end &&
           start < b.EndTime);
        }
        public async Task<int> UsedSeatsAsync(int tableId, DateTime start, DateTime end, int? excludeBookingId = null)
        {
          return await _context.Bookings
                .Where(b => b.FK_TableId == tableId &&
                (excludeBookingId == null || b.Id != excludeBookingId.Value) &&
                b.StartTime < end &&
                start < b.EndTime)
                .SumAsync(b => (int?)b.PartySize)
                .ContinueWith(t => t.Result ?? 0);

        }
        public async Task<int> AddBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking.Id;
        }

        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            var rowsAffected = await _context.Bookings
                .Where(b => b.Id == bookingId)
                .ExecuteDeleteAsync();
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }   
           

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
           
            return await _context.Bookings
                .Include(b => b.Table)
                .Include(b => b.Guest)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {

            return await _context.Bookings
              .Include(b => b.Table)
                .Include(b => b.Guest)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

    }
}

