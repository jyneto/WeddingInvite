using WeddingInvite.Api.DTOs.TableDTO;
namespace WeddingInvite.Api.Services.Interfaces
{
    public interface ITableService
    {
        Task<List<TableGetDTO>> GetAllTableAsync();
        Task<TableGetDTO?> GetTableByIdAsync(int id);
        Task<int> AddTableAsync(TableCreateDTO table);
        Task<bool> UpdateTableAsync(TableUpdateDTO table);
        Task<bool> DeleteTableAsync(int id);
    }
}
