using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.DTOs.AuthDTO;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Controllers
{

    [Route("api[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginDTO adminLoginDTO)
        {
            var adminResponse = await authService.LoginAsync(adminLoginDTO);
            if (adminResponse == null)
            {
                return Unauthorized("Invalid username or password");
            }
            return Ok(adminResponse);
        }
    }
}
