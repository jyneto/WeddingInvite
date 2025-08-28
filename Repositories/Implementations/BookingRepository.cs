using Microsoft.EntityFrameworkCore;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;
namespace WeddingInvite.Api.Repositories.Implementations
{
    public class BookingRepository : IBookingRespository
    {
        private readonly WeddingContext _context;
        public BookingRepository(WeddingContext context)
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

        public Task<bool> UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            var result = _context.SaveChangesAsync();
            if (result.Result > 0)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
