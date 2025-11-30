using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.DTOs.BookingDTO;
using WeddingInvite.Api.DTOs.GuestDTO;
using WeddingInvite.Api.DTOs.TableDTO;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Controllers
{
    [ApiController]
    //[Route("api/guest")]
    [Route("api/[controller]")]
    //[Route("api/")]

    [Authorize(Roles = "Admin,User")]
    public class GuestController : ControllerBase
    {
        private readonly IGuestService _guestService;
        private readonly IBookingService _bookingService;
        private readonly WeddingDbContext _context;
        public GuestController(IGuestService guestService, IBookingService bookingService, WeddingDbContext context)
        {
            _guestService = guestService;
            _bookingService = bookingService;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SubmitRSVP([FromBody] GuestCreateDTO guestCreateDto)
        {
            Console.WriteLine($"DTO in: IsAttending={guestCreateDto.IsAttending}, TableId={guestCreateDto.TableId}");


            try
            {
                var id = await _guestService.AddGuestAsync(guestCreateDto);
                return CreatedAtAction(nameof(GetGuestById), new { id = id }, new { Id = id });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Conflict($"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGuestById(int id)
        {
            var guest = await _guestService.GetGuestByIdAsync(id);
            if (guest == null)
            {
                return NotFound();
            }
            return Ok(guest);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllbyIdAsync()
        {
            var guest = await _guestService.GetAllGuestAsync();
            if (guest == null)
            {
                return NotFound();
            }
            return Ok(guest);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]

        public async Task<IActionResult> UpdateGuest(int Id, [FromBody] GuestUpdateDTO guestUpdateDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (Id != guestUpdateDTO.Id) return BadRequest("ID missmatch");

            var guest = await _guestService.GetGuestByIdAsync(Id);
            if (guest == null) return NotFound();

            guest.Id = guestUpdateDTO.Id;
            guest.IsAttending = guestUpdateDTO.IsAttending;
            guest.Allergies = guestUpdateDTO.Allergies;
            guest.TableId = guestUpdateDTO.TableId;
            await _guestService.UpdateGuestAsync(Id, guestUpdateDTO);
            return NoContent();
        }

      
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _guestService.DeleteGuestAsync(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

    }
}
