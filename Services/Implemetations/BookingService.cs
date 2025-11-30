using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using WeddingInvite.Api.DTOs.BookingDTO;
using WeddingInvite.Api.DTOs.TableDTO;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;
using WeddingInvite.Api.Services.Interfaces;
using AvailableTableDTO = WeddingInvite.Api.DTOs.TableDTO.AvailableTableDTO;
//using AvailableTableDTO = WeddingInvite.Api.DTOs.BookingDTO.AvailableTableDTO;

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
                throw new ArgumentException("Maximum party is reached size is maximum 10 guest.");

            
            var start = bookingRequestDTO.StartTime.Kind == DateTimeKind.Utc ? bookingRequestDTO.StartTime : bookingRequestDTO.StartTime.ToUniversalTime();
            var end = start.Add(Duration);
            //overlap
            if (start.Date != EventDate.Date)
                throw new ArgumentException($"Bookings must be on {EventDate:yyyy-MM-dd}.");

            //if (await _bookingRepo.BookingOverlapAsync(bookingRequestDTO.TableId, start, end))
            //{
            //    throw new InvalidOperationException("Booking time overlaps with an existing booking.");
            //}

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
         
            //if (guest != null)
            //{
            //    guest.TableId = bookingRequestDTO.TableId;
            //    await _guestRepo.UpdateGuestAsync(guest);
            //}

            return bookingId;
        }

        public async Task<int> AddRsvpWithBookingAsync(RsvpBookingRequestDTO dto)
        {
            //1. Get table
            var table = await _tableRepo.GetByIdAsync(dto.TableId);
            if (table == null)
                throw new ArgumentException($"Table with ID {dto.TableId} does not exist.");
            if (dto.PartySize > table.Capacity)
                throw new ArgumentException($"Table {table.TableNumber} vannot accomodate party size {dto.PartySize}.");
            if (dto.PartySize > 10)
                throw new ArgumentException("Maximum party size per booking is 10.");

            //2. Normalize time and validate date + overlap + capicity

            var start = dto.StartTime.Kind == DateTimeKind.Utc 
                ? dto.StartTime 
                : dto.StartTime.ToUniversalTime();

            var end = start.Add(Duration);

            //Date check - only allow bookings on the weddíng date
            if (start.Date != EventDate.Date)
                throw new ArgumentException($"Bookings must be on {EventDate:yyyy-MM-dd}.");

            //Capacity based on existing bookings
            //if(await _bookingRepo.BookingOverlapAsync(dto.TableId, start, end))
            //    throw new InvalidOperationException("Booking time overlaps with an existing booking.");

            //Capacity check based on existing bookings
            var used = await _bookingRepo.UsedSeatsAsync(dto.TableId, start, end);
            var remaining = table.Capacity - used;

            if(dto.PartySize > remaining)
                throw new InvalidOperationException(remaining <= 0 
                    ? "Table is fully booked" 
                    : $"Only {remaining} seats are left on this table."
                );

            //After checks and validation -> Create Guest

            var email = dto.Email.Trim().ToLowerInvariant();
            if(string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            if(await _guestRepo.EmailExistAsync(email))
                throw new ArgumentException("A guest with this email already exists.");

            var guest = new Guest
            {
                FullName = dto.FullName?.Trim() ?? string.Empty,
                Email = email,
                Phone = dto.Phone?.Trim(),
                IsAttending = true,
                Allergies = dto.Allergies?.Trim(),
                TableId = dto.TableId
            };

            await _guestRepo.AddGuestAsync(guest);

            //4. Create Booking 
            var booking = new Booking
            {
                FK_TableId = dto.TableId,
                FK_GuestId = guest.Id,
                StartTime = start,
                EndTime = end,
                PartySize = dto.PartySize
            };

            var bookingId = await _bookingRepo.AddBookingAsync(booking);
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
                TableNumber =b.Table.TableNumber,
                GuestId = b.FK_GuestId,
                GuestName = b.Guest != null ? b.Guest.FullName : null,
                StartTime = b.StartTime,
                EndTime = b.EndTime,    
                PartySize = b.PartySize
            }).ToList();
            return bookingDTOs;

        }

        //public async Task<List<AvailableTableDTO>> GetAvailableTablesAsync(AvailabilityRequestDTO availabilityRequestDTO)
        //{
        //    var start = DateTime.SpecifyKind(availabilityRequestDTO.StartTime, DateTimeKind.Utc);
        //    var end = start.Add(Duration);

        //    var tables = await _tableRepo.GetAllTableAsync();
        //    var bookings = await _bookingRepo.GetAllBookingsAsync();


        //    var unavailableTableIds = bookings
        //        .Where (b => b.StartTime < end && start < b.EndTime)
        //        .Select(b => b.FK_TableId)
        //        .ToHashSet();

        //    var availableTables = tables
        //        .Where (t => t.Capacity >= availabilityRequestDTO.PartySize && !unavailableTableIds.Contains(t.Id))
        //        .Select (t => new AvailableTableDTO
        //        {
        //            TableId = t.Id,
        //            TableNumber = t.TableNumber,
        //            Capacity = t.Capacity
        //        }).ToList();
        //    return availableTables;
        //}



        //public async Task<List<AvailableTableDTO>> GetAvailableTablesAsync(AvailabilityRequestDTO dto)
        //    {
        //        // Normalize to UTC and compute end of the slot
        //        var start = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc);
        //        var end = start.Add(Duration);

        //        var tables = await _tableRepo.GetAllTableAsync();
        //        var result = new List<AvailableTableDTO>(tables.Count);

        //        foreach (var table in tables)
        //        {
        //            // quick reject: table too small for the requested party
        //            if (table.Capacity < dto.PartySize) continue;

        //            // seats already taken in this time window (sum of PartySize for overlapping bookings)
        //            // NOTE: this is the overload without "excludeBookingId"
        //            var used = await _bookingRepo.UsedSeatsAsync(table.Id, start, end);

        //            var remaining = table.Capacity - used;
        //            if (remaining >= dto.PartySize)
        //            {
        //                result.Add(new AvailableTableDTO
        //                {
        //                    TableId = table.Id,
        //                    TableNumber = table.TableNumber,
        //                    Capacity = table.Capacity,
        //                    AvailableSeats = remaining
        //                });
        //            }
        //        }

        //        return result
        //            .OrderByDescending(x => x.AvailableSeats)
        //            .ThenBy(x => x.TableNumber)
        //            .ToList(); 
        //     }

        public async Task<List<AvailableTableDTO>> GetAvailableTablesAsync(AvailabilityRequestDTO dto)
        {
            // 1. Normalize start time and compute end of slot
            var start = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc);
            var end = start.Add(Duration); // Duration = TimeSpan.FromMinutes(_policy.DurationMinutes)

            // 2. Load all tables
            var tables = await _tableRepo.GetAllTableAsync();

            // 3. Load all bookings that overlap this time window
            //    (instead of relying on UsedSeatsAsync)
            var allBookings = await _bookingRepo.GetAllBookingsAsync();

            var relevantBookings = allBookings
                .Where(b => b.StartTime < end && start < b.EndTime)
                .ToList();

            var result = new List<AvailableTableDTO>();

            foreach (var table in tables)
            {
                // Skip tables that are too small for the requested party
                if (table.Capacity < dto.PartySize)
                    continue;

                // 4. Seats already used at this table in this time window
                var usedSeats = relevantBookings
                    .Where(b => b.FK_TableId == table.Id)
                    .Sum(b => b.PartySize);

                var remaining = table.Capacity - usedSeats;

                // 5. Only include tables that can actually fit this party
                if (remaining >= dto.PartySize)
                {
                    result.Add(new AvailableTableDTO
                    {
                        TableId = table.Id,
                        TableNumber = table.TableNumber,
                        Capacity = table.Capacity,
                        AvailableSeats = remaining
                    });
                }
            }

            // 6. Sort results nicely
            return result
                .OrderByDescending(x => x.AvailableSeats)
                .ThenBy(x => x.TableNumber)
                .ToList();
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

        public static readonly DateTime EventDate = new DateTime(2026, 8, 23); //Wedding date
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
            //if (await _bookingRepo.BookingOverlapAsync(bookingRequestDTO.TableId, start, end)) throw new InvalidOperationException("Booking time overlaps with another booking in this table.");

            
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
