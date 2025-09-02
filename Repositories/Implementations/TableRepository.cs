using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;

namespace WeddingInvite.Api.Repositories.Implementations
{
    public class TableRepository : ITableRespiratory
    {
        private readonly WeddingDbContext _context;
        public TableRepository(WeddingDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddTableAsync(Table table)
        {
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();
            return table.Id;
        }

        public async Task<bool> DeleteTableAsync(int tableId)
        {
            var rowsAffected = await _context.Tables
                .Where(t => t.Id == tableId)
                .ExecuteDeleteAsync();
            if(rowsAffected > 0) 
            {
                return true;
            }
            return false;
        }

        public async Task<Table?> GetByIdAsync(int id)
        {
            return await _context.Tables.FindAsync(id);
        }

        public async Task<List<Table>> GetAllTableAsync()
        {
            var table = await _context.Tables.ToListAsync();
            return table;  
        }

        public async Task<Table?> GetTableByIdAsync(int id)
        {
            var table = await _context.Tables
                .FirstOrDefaultAsync(t => t.Id == id);
            return table;
        }

        public Task<bool> UpdateTableAsync(Table table)
        {
            _context.Tables.Update(table);
            var result =  _context.SaveChangesAsync();
            if(result.Result > 0)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
