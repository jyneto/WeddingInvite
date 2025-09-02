using Microsoft.AspNetCore.Mvc;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.DTOs.BookingDTO;
using WeddingInvite.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace WeddingInvite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin")]
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
            var createdBookingId = await _bookingService.AddBookingAsync(bookingCreateDTO);
            var booking = await _bookingService.GetBookingByIdAsync(createdBookingId);
            return CreatedAtAction(nameof(GetBookingById), new { id = createdBookingId }, booking);
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

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingGetDTO bookingUpdateDTO)
        {
            if (id != bookingUpdateDTO.Id)
            {
                return BadRequest("ID mismatch");
            }
            var result = await _bookingService.UpdateBookingAsync(id, bookingUpdateDTO);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
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

        [HttpPost("available")]
        public async Task<IActionResult> GetAvailableTables([FromBody] AvailabilityRequestDTO availabilityDTO)
        {
            var availableTables = await _bookingService.GetAvailableTablesAsync(availabilityDTO);
            return Ok(availableTables);
        }

    }
}
