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
        {
            var guest = await _guestRepo.GetByIdAsync(bookingRequestDTO.GuestId);
            var table = await _tableRepo.GetByIdAsync(bookingRequestDTO.TableId);
            if (guest == null) throw new ArgumentException($"Guest with ID {bookingRequestDTO.GuestId} does not exist.");
            if (table == null) throw new ArgumentException($"Table with ID {bookingRequestDTO.TableId} does not exist.");

            if (bookingRequestDTO.PartySize > table.Capacity) 
                throw new ArgumentException($"Table {table.TableNumber} cannot accommodate party size {bookingRequestDTO.PartySize}.");
            if (bookingRequestDTO.PartySize > 10) 
                throw new ArgumentException("Maximum party is size is maximum 4 guest.");

            
            var start = bookingRequestDTO.StartTime.Kind == DateTimeKind.Utc ? bookingRequestDTO.StartTime : bookingRequestDTO.StartTime.ToUniversalTime();
            var end = start.Add(Duration);
            //overlap
            if (start.Date != EventDate.Date)
                throw new ArgumentException($"Bookings must be on {EventDate:yyyy-MM-dd}.");
            if (await _bookingRepo.BookingOverlapAsync(bookingRequestDTO.TableId, start, end))
            {
                throw new InvalidOperationException("Booking time overlaps with an existing booking.");
            }

            //capacity
            var used = await _bookingRepo.UsedSeatsAsync(bookingRequestDTO.TableId, start, end);
            var remaining = table.Capacity - used;
            if (bookingRequestDTO.PartySize > remaining) 
                throw new InvalidOperationException(remaining <= 0 ? "Table is fully booked" : $"Only {remaining} seats are left on this table");

            var booking = new Booking
            {
                FK_TableId = bookingRequestDTO.TableId,
                FK_GuestId = bookingRequestDTO.GuestId,
                StartTime = start,
                EndTime = end,
                PartySize = bookingRequestDTO.PartySize
            };
            // Create the booking
            var bookingId = await _bookingRepo.AddBookingAsync(booking);
         
            if (guest != null)
            {
                guest.TableId = bookingRequestDTO.TableId;
                await _guestRepo.UpdateGuestAsync(guest);
            }

            return bookingId;
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

        public static readonly DateTime EventDate = new DateTime(2026, 8, 15); //Wedding date
        public async Task<bool> UpdateBookingAsync(int id,BookingGetDTO bookingRequestDTO)
        {
            var existingBooking = await _bookingRepo.GetByIdAsync(id);
            if (existingBooking == null) return false;

            // validate new table & capacity
            var table = await _tableRepo.GetByIdAsync(bookingRequestDTO.TableId);
            if(table == null) 
                throw new ArgumentException($"Table with ID {bookingRequestDTO.TableId} does not exist.");

            if (bookingRequestDTO.PartySize > table.Capacity) 
                throw new ArgumentException("Maximum party  size per booiking is 4");
            if (bookingRequestDTO.PartySize > 10) throw new ArgumentException("Maximum party size per booking is 4.");

            var start = DateTime.SpecifyKind(bookingRequestDTO.StartTime, DateTimeKind.Utc);
            var end = start.Add(Duration);
            if (start.Date != EventDate.Date) 
                throw new ArgumentException($"Bookings must be on {EventDate:yyyy-MM-dd}.");
            //Overlap
            if (await _bookingRepo.BookingOverlapAsync(bookingRequestDTO.TableId, start, end)) throw new InvalidOperationException("Booking time overlaps with another booking in this table.");

            
            var used = await _bookingRepo.UsedSeatsAsync(bookingRequestDTO.TableId, start, end, id);
            var remaining = table.Capacity - used;

            if(bookingRequestDTO.PartySize > remaining) 
                throw new InvalidOperationException(remaining <= 0 ? "Table is fully booked" : $"Only {remaining} seats are left on this table.");

            existingBooking.FK_TableId = bookingRequestDTO.TableId;
            existingBooking.FK_GuestId = bookingRequestDTO.GuestId;
            existingBooking.StartTime = start;
            existingBooking.EndTime = end;
            existingBooking.PartySize = bookingRequestDTO.PartySize;

            return await _bookingRepo.UpdateBookingAsync(existingBooking);
        }
    }
}
