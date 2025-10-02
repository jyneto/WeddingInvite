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

       public Task<bool> ExistsAsync(int id) =>
            _context.Tables.AnyAsync(t => t.Id == id);
        
        public async Task<bool> IsInUseAsync(int tableId)
        {
            return await _context.Guests.AnyAsync(g => g.TableId == tableId) || await _context.Bookings.AnyAsync(b => b.FK_TableId == tableId);
        }
        public async Task<bool> DeleteTableAsync(int tableId)
        {
            var table = await _context.Tables.FindAsync(tableId);
            if (table is null) return false;
            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
