using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.DTOs.AuthDTO;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Services.Implemetations
{
    public class AuthService : IAuthService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IConfiguration _configuration;
        public AuthService(IAdminRepository adminRepository, IConfiguration configuration)
        {
            _adminRepository = adminRepository;
            _configuration = configuration;
        }

        public async Task<bool> RegisterAsync(AdminRegisterDTO adminRegisterDTO)
        {
            var usernameTaken = await _adminRepository.UserNameExistsAsync(adminRegisterDTO.UserName);
            if (usernameTaken)
                return false;
            var hashedPassword = await HashPassword(adminRegisterDTO.Password!);
            var newAdmin = new Admin
            {
                UserName = adminRegisterDTO.UserName,
                PasswordHash = hashedPassword
            };
            return await _adminRepository.AddAdminAsync(newAdmin);
        }
        public async Task<AdminResponseDTO?> LoginAsync(AdminLoginDTO adminLoginDTO)
        {
            var admin = await _adminRepository.GetByUserNameAsync( adminLoginDTO.Username!);
            if (admin == null || !BCrypt.Net.BCrypt.Verify(adminLoginDTO.Password!, admin.PasswordHash!))
                return null;

            var token = GenerateJwtToken(admin);
            var response = new AdminResponseDTO
            {
                Username = admin.UserName,
                Token = token
            };
            return response;
        }
        public Task<string> HashPassword(string password)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password));
        }

        public Task<bool> VerifyPassword(string password, string hash)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hash));
        }
        private string GenerateJwtToken(Admin admin)
        {
            var claims = new[]
            {
               new Claim(ClaimTypes.Name, admin.UserName!),
               new Claim (ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
