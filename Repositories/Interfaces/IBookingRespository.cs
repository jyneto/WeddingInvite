using WeddingInvite.Api.Models;

namespace WeddingInvite.Api.Repositories.Interfaces
{
    public interface IBookingRespository
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task <int>AddBookingAsync(Booking booking);
        Task<bool> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int bookingId);
    }
}
