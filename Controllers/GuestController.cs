using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingInvite.Api.DTOs.GuestDTO;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Controllers
{
    [ApiController]
    [Route("api/guest/rsvp")]
    public class GuestController : ControllerBase
    {
        private IGuestService _guestService;
        public GuestController(IGuestService guestService)
        {
            _guestService = guestService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SubmitRSVP([FromBody] GuestCreateDTO guestCreateDto)
        {
            if (!ModelState.IsValid)
            return BadRequest(ModelState);

            try
            {
                var id = await _guestService.AddGuestAsync(guestCreateDto);
                return CreatedAtAction(nameof(GetGuestById), new { id = id }, new { Id = id });
            }
            catch (ArgumentException ex)
            { 
                return BadRequest(ex.Message );
            }
            catch (DbUpdateException)
            {
                return Conflict("A guest with this email already exist.");
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
    }
}
