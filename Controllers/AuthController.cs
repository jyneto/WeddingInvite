using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.DTOs.AuthDTO;
using WeddingInvite.Api.DTOs.BookingDTO;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;
using WeddingInvite.Api.Services.Implemetations;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {

            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginDTO adminLoginDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var adminResponse = await _authService.LoginAsync(adminLoginDTO);
            if (adminResponse == null)
            {
                return Unauthorized("Invalid username or password");
            }
            return Ok(adminResponse);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AdminRegisterDTO adminRegisterDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _authService.RegisterAsync(adminRegisterDTO);
            if (!created)
            {
                return Conflict("Username already exists");
            }
            return StatusCode(201);
        }

        //Hjälp endpoint för att verifiera  token/claims i Swagger
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindAll(ClaimTypes.Role).Select(r => r.Value);
            return Ok(new { Id = id, Username = username, Role = role });


        }
    }
}
