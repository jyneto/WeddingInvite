using Microsoft.Extensions.Options;
using WeddingInvite.Api.DTOs.BookingDTO;
using WeddingInvite.Api.DTOs.TableDTO;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Services.Implemetations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRespository _bookingRepo;
        private readonly ITableRespiratory _tableRepo;
        private readonly IGuestRepository _guestRepo;
        private readonly EventPolicy _policy;

        public BookingService(IBookingRespository bookingRepo, ITableRespiratory tableRepo, IGuestRepository guestRepo, IOptions<EventPolicy> policy)
        {
            _bookingRepo = bookingRepo;
            _tableRepo = tableRepo;
            _guestRepo = guestRepo;
            _policy = policy.Value;
        }

        private TimeSpan Duration => TimeSpan.FromMinutes(_policy.DurationMinutes);
        public async Task<int> AddBookingAsync(BookingCreateDTO bookingRequestDTO)
        {   var guest = await _guestRepo.GetByIdAsync(bookingRequestDTO.GuestId);
            var start = DateTime.SpecifyKind(bookingRequestDTO.StartTime, DateTimeKind.Utc);
            var end = start.Add(Duration);

            if (guest == null)
            {
                throw new ArgumentException($"Guest with ID {bookingRequestDTO.GuestId} does not exist.");
            }
            var table = await _tableRepo.GetByIdAsync(bookingRequestDTO.TableId);
            if (table == null)
            {
                throw new ArgumentException($"Table with ID {bookingRequestDTO.TableId} does not exist.");
            }
            if (bookingRequestDTO.PartySize > table.Capacity)
            {
                throw new ArgumentException($"Table with ID {bookingRequestDTO.TableId} cannot accommodate party size of {bookingRequestDTO.PartySize}.");
            }
            if (bookingRequestDTO.PartySize > 4)
            {
                throw new ArgumentException("Maximum party size per booking is 4 guest.");
            }

            if (await _bookingRepo.BookingOverlapAsync(table.Id, start, end))
            {
                throw new InvalidOperationException("Booking time overlaps with an existing booking.");
            }

            var newBooking = new Booking
            {
                FK_TableId = bookingRequestDTO.TableId,
                FK_GuestId = bookingRequestDTO.GuestId,
                StartTime = DateTime.SpecifyKind(bookingRequestDTO.StartTime, DateTimeKind.Utc),
                EndTime = DateTime.SpecifyKind(bookingRequestDTO.StartTime, DateTimeKind.Utc).Add(Duration),
                PartySize = bookingRequestDTO.PartySize
            };
            return await _bookingRepo.AddBookingAsync(newBooking);
        }


        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null)
            {
                return false;
            }
            await _bookingRepo.DeleteBookingAsync(booking.Id);
            return true;
        }

        public async Task<List<BookingGetDTO>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepo.GetAllBookingsAsync();
            var bookingDTOs = bookings.Select(b => new BookingGetDTO
            {
                Id = b.Id,
                TableId = b.FK_TableId,
                GuestId = b.FK_GuestId,
                StartTime = b.StartTime,
                EndTime = b.EndTime,    
                PartySize = b.PartySize
            }).ToList();
            return bookingDTOs;

        }

        public async Task<List<AvailableTableDTO>> GetAvailableTablesAsync(AvailabilityRequestDTO availabilityRequestDTO)
        {
            var start = DateTime.SpecifyKind(availabilityRequestDTO.StartTime, DateTimeKind.Utc);
            var end = start.Add(Duration);

            var tables = await _tableRepo.GetAllTableAsync();
            var bookings = await _bookingRepo.GetAllBookingsAsync();


            var unavailableTableIds = bookings
                .Where (b => b.StartTime < end && start < b.EndTime)
                .Select(b => b.FK_TableId)
                .ToHashSet();

            var availableTables = tables
                .Where (t => t.Capacity >= availabilityRequestDTO.PartySize && !unavailableTableIds.Contains(t.Id))
                .Select (t => new AvailableTableDTO
                {
                    TableId = t.Id,
                    TableNumber = t.TableNumber,
                    Capacity = t.Capacity
                }).ToList();
            return availableTables;
        }

        public async Task<BookingGetDTO?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null)
            {
                return null;
            }
            var bookingDto = new BookingGetDTO
            {
                Id = booking.Id,
                TableId = booking.FK_TableId,
                GuestId = booking.FK_GuestId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                PartySize = booking.PartySize
            };
            return bookingDto;

        }

        public async Task<bool> UpdateBookingAsync(int id, BookingGetDTO bookingRequestDTO)
        {
            var existingBooking = await _bookingRepo.GetByIdAsync(id);
            if (existingBooking == null) return false;

            existingBooking.FK_TableId = bookingRequestDTO.TableId;
            existingBooking.FK_GuestId = bookingRequestDTO.GuestId;
            existingBooking.StartTime = DateTime.SpecifyKind(bookingRequestDTO.StartTime, DateTimeKind.Utc);
            existingBooking.EndTime = DateTime.SpecifyKind(bookingRequestDTO.StartTime, DateTimeKind.Utc).Add(Duration);
            existingBooking.PartySize = bookingRequestDTO.PartySize;
            return await _bookingRepo.UpdateBookingAsync(existingBooking);
        }
    }
}
