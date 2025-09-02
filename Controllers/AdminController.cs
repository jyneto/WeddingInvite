using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingInvite.Api.DTOs.AuthDTO;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AdminController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AdminRegisterDTO adminRegisterDTO)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var existingAdmin = await _authService.RegisterAsync(adminRegisterDTO);
            if (!existingAdmin)
            {
                return Conflict("Username already exists");
            }
            return Ok("Admin register complete");
        }
    }
}
