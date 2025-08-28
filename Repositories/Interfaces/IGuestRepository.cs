using WeddingInvite.Api.Models;
namespace WeddingInvite.Api.Repositories.Interfaces
{
    public interface IGuestRepository
    {
        Task<List<Guest>> GetAllGuestAsync();
        Task<Guest?> GetByIdAsync(int id);
        Task <int> AddGuestAsync(Guest guest);
        Task <bool> UpdateGuestAsync(Guest guest);
        Task <bool> DeleteGuestAsync(int guestId);  
    }
}
