using WeddingInvite.Api.DTOs.TableDTO;
using WeddingInvite.Api.Models;

namespace WeddingInvite.Api.Repositories.Interfaces
{
    public interface ITableRespiratory
    {
        Task<List<Table>> GetAllTableAsync();
        Task<Table?> GetTableByIdAsync(int id);
        Task<int> AddTableAsync(Table table);
        Task<bool> UpdateTableAsync(Table table);
        Task<bool> DeleteTableAsync(int tableId);
    }
}
