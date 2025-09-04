using WeddingInvite.Api.Models;
using WeddingInvite.Api.DTOs.GuestDTO;

namespace WeddingInvite.Api.Services.Interfaces
{
    public interface IGuestService
    {
        Task<List<GuestGetDTO>> GetAllGuestAsync();
        Task<GuestGetDTO?> GetGuestByIdAsync(int id);
        Task<int> AddGuestAsync(GuestCreateDTO guestCreateDto);
        Task<bool> UpdateGuestAsync(int id, GuestUpdateDTO guest);
        Task<bool> DeleteGuestAsync(int guestId);
        Task<bool> EmailExistAsync(string email);

    }
}
