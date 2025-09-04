using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingInvite.Api.DTOs.GuestDTO;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Controllers
{
    [ApiController]
    [Route("api/admin/guest")]
    [Authorize(Roles = "Admin")]
    public class AdminGuestController : ControllerBase
    {
        private readonly IGuestService _guestService;
        public AdminGuestController(IGuestService guestService)
        {
            _guestService = guestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGuests()
        {
            var guests = await _guestService.GetAllGuestAsync();
            return Ok(guests);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGuestById(int id)
        {
            var guestId = await _guestService.GetGuestByIdAsync(id);
            if (guestId == null)
            {
                return NotFound();
            }
            return Ok(guestId);
        }
        [HttpPost]
        public async Task<IActionResult> AddGuest([FromBody] GuestCreateDTO guestCreateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            guestCreateDTO.FullName = guestCreateDTO.FullName?.Trim();
            guestCreateDTO.Email = guestCreateDTO.Email?.Trim().ToLowerInvariant();
            guestCreateDTO.Phone = guestCreateDTO.Phone?.Trim();

            if (await _guestService.EmailExistAsync(guestCreateDTO.Email!))
            {
                return Conflict("A guest with this email already exists.");
            }
            try
            {
                var id = await _guestService.AddGuestAsync(guestCreateDTO);
                return CreatedAtAction(nameof(GetGuestById), new { id = id }, new { Id = id });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("email", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict("A guest with this email already exist.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateGuest(int id, [FromBody] GuestUpdateDTO guestUpdateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
     

            var isUpdated = await _guestService.UpdateGuestAsync(id, guestUpdateDTO);
            if (!isUpdated)
            {
                return NotFound("Guest not found, update failed");
            }
            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            var isDeleted = await _guestService.DeleteGuestAsync(id);
            if (!isDeleted)
            {
                return NotFound("Guest not found, delete failed");
            }
            return NoContent();
        }

    }
}
