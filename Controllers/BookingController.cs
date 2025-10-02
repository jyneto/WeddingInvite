using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingInvite.Api.DTOs.BookingDTO;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Services.Implemetations;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize(Roles = "Admin")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDTO bookingCreateDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var createdBookingId = await _bookingService.AddBookingAsync(bookingCreateDTO);
                var bookingCreated = await _bookingService.GetBookingByIdAsync(createdBookingId);
                return CreatedAtAction(nameof(GetBookingById), new { id = createdBookingId }, bookingCreated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException)
            {
                return Conflict("Could not create the booking due to a data conflict");
            }

        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllBookingsAsync()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            if (bookings == null)
            {
                return NotFound();
            }
            return Ok(bookings);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingGetDTO bookingUpdateDTO)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != bookingUpdateDTO.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var result = await _bookingService.UpdateBookingAsync(id, bookingUpdateDTO);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException)
            {
                return Conflict("Could not update the booking due to a data conflict");
            }
        }


        [AllowAnonymous]
        [HttpPost("available")]
        public async Task<IActionResult> GetAvailableTables([FromBody] AvailabilityRequestDTO availabilityDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var availableTables = await _bookingService.GetAvailableTablesAsync(availabilityDTO);
            return Ok(availableTables);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var result = await _bookingService.DeleteBookingAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
