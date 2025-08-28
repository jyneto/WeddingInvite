using Microsoft.AspNetCore.Mvc;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.DTOs.BookingDTO;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
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



    }
}
