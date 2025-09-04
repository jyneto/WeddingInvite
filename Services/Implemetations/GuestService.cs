using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.DTOs.GuestDTO;
using WeddingInvite.Api.Services.Interfaces;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;
namespace WeddingInvite.Api.Services.Implemetations
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _guestRepo;
        public GuestService(IGuestRepository guestRepo)
        {
            _guestRepo = guestRepo;
        }
        public Task<bool> EmailExistAsync(string email) =>

            _guestRepo.EmailExistAsync(email);


        public async Task<int> AddGuestAsync(GuestCreateDTO guestCreateDTO)
        {
            var email= guestCreateDTO.Email?.Trim().ToLowerInvariant();
            if(await _guestRepo.EmailExistAsync(email!))
                throw new InvalidOperationException("A guest with the same email already exists.");

            var guest = new Guest
            {
                FullName = guestCreateDTO.FullName?.Trim(),
                Email = guestCreateDTO.Email!,
                Phone = guestCreateDTO.Phone?.Trim(),
                IsAttending = guestCreateDTO.IsAttending,
                Allergies = guestCreateDTO.Allergies?.Trim()
            };
            return await _guestRepo.AddGuestAsync(guestCreateDTO);
        }

        public async Task<List<GuestGetDTO>> GetAllGuestAsync()
        {
            var guests = await _guestRepo.GetAllGuestAsync();
            return guests.Select(g => new GuestGetDTO
            {
                Id = g.Id,
                FullName = g.FullName,
                Email = g.Email,
                Phone = g.Phone,
                IsAttending = g.IsAttending,
                Allergies = g.Allergies
            }).ToList();
        }

        public async Task<GuestGetDTO?> GetGuestByIdAsync(int id)
        {
            var guest = await _guestRepo.GetByIdAsync(id);
               
                if (guest == null)
                {
                    return null;
                }
            var  guesDTos = new GuestGetDTO
            {
                Id = guest.Id,
                FullName = guest.FullName,
                Email = guest.Email,
                Phone = guest.Phone,
                IsAttending = guest.IsAttending,
                Allergies = guest.Allergies
            };

            return guesDTos;
        }

        public async Task<bool> UpdateGuestAsync(int id, GuestUpdateDTO guestUpdateDTO)
        {
            var existingGuest = await _guestRepo.GetByIdAsync(id);
            if (existingGuest == null)
            {
                return false;
            }
            existingGuest.FullName = guestUpdateDTO.FullName;
            existingGuest.Email = guestUpdateDTO.Email;
            existingGuest.Phone = guestUpdateDTO.Phone;
            existingGuest.IsAttending = guestUpdateDTO.IsAttending;
            existingGuest.Allergies = guestUpdateDTO.Allergies;
        
            return await _guestRepo.UpdateGuestAsync(existingGuest);

        }
        public async Task<bool> DeleteGuestAsync(int id)
        {
            return await _guestRepo.DeleteGuestAsync(id);
        }


    }
}
