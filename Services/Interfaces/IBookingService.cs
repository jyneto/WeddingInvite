using WeddingInvite.Api.DTOs.BookingDTO;
using WeddingInvite.Api.DTOs.TableDTO;

namespace WeddingInvite.Api.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingGetDTO>> GetAllBookingsAsync();
        Task<BookingGetDTO?> GetBookingByIdAsync(int id);
        Task<int> AddBookingAsync(BookingCreateDTO bookingRequestDTO);
        Task<bool> UpdateBookingAsync(int id, BookingGetDTO bookingRequestDTO);
        Task<bool> DeleteBookingAsync(int id);
        Task<List<AvailableTableDTO>> GetAvailableTablesAsync(AvailabilityRequestDTO availabilityRequestDTO);
        Task<int> AddRsvpWithBookingAsync(RsvpBookingRequestDTO dto);
    }
}
