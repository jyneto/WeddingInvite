using WeddingInvite.Api.DTOs.TableDTO;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Services.Implemetations
{
    public class TableService : ITableService
    {
        private readonly ITableRespiratory _tableRepo;
        public TableService(ITableRespiratory tableRespiratory)
        {
            _tableRepo = tableRespiratory;
        }

        public async Task<int> AddTableAsync(TableCreateDTO table)
        {
            var newTable = new Table
            {
                TableNumber = table.TableNumber,
                Capacity = table.Capacity
            };
            return await _tableRepo.AddTableAsync(newTable);
        }

        public async Task<bool> DeleteTableAsync(int tableId)
        {
            var tableFound = await _tableRepo.GetTableByIdAsync(tableId);
            if (tableFound == null)
            {
                return false;
            }
            await _tableRepo.DeleteTableAsync(tableId);
            return true;

        }

        public async Task<List<TableGetDTO>> GetAllTableAsync()
        {
            var tables = await _tableRepo.GetAllTableAsync();
            var tableDTOs = tables.Select(t => new TableGetDTO
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                Capacity = t.Capacity
            }).ToList();
            return tableDTOs;
        }

        public async Task<TableGetDTO?> GetTableByIdAsync(int id)
        {
            var table =  await _tableRepo.GetTableByIdAsync(id);

            if(table == null)
            {
                return null;
            }

            var tableDto = new TableGetDTO
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity
            };
            return tableDto;

        }

        public async Task<bool> UpdateTableAsync(TableUpdateDTO tableUpdateDTO)
        {
           var existingTable = await _tableRepo.GetTableByIdAsync(tableUpdateDTO.Id);
            if(existingTable == null)
            {
                return false;
            }

            existingTable.TableNumber = tableUpdateDTO.TableNumber;
            existingTable.Capacity = tableUpdateDTO.Capacity;
            await _tableRepo.UpdateTableAsync(existingTable);
            return true;


        }
    }
}
