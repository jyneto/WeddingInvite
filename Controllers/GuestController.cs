using Microsoft.AspNetCore.Mvc;
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
        [HttpPost]
        public async Task<IActionResult> SubmitRSVP([FromBody] GuestCreateDTO guestCreateDTO)
        {
            if (!ModelState.IsValid)
            return BadRequest(ModelState);

            var newGuestId = await _guestService.AddGuestAsync(guestCreateDTO);
            return CreatedAtAction(nameof(SubmitRSVP), new { id = newGuestId }, new { Id = newGuestId });

        }

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
