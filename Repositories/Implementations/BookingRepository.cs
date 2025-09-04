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
            var bookings = await _context.Bookings.ToListAsync();
            return bookings;
        }

        public Task<Booking?> GetByIdAsync(int id)
        {   
            var booking =  _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);
            return booking;
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> BookingOverlapAsync(int bookingId, DateTime start, DateTime end)
        {
            //return await _context.Bookings
            //    .Where(b => b.FK_TableId == bookingId)
            //    .AnyAsync(b => b.StartTime < end && start < b.EndTime);

            return await _context.Bookings
                .AnyAsync(b => 
                b.FK_TableId == bookingId && 
                b.StartTime < end && 
                start < b.EndTime);
        }
    }
}

