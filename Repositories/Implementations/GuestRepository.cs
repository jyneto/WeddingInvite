﻿using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.DTOs.GuestDTO;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;

namespace WeddingInvite.Api.Repositories.Implementations
{
    public class GuestRepository : IGuestRepository
    {
        private readonly WeddingDbContext _context;
        public GuestRepository(WeddingDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddGuestAsync(GuestCreateDTO guest)
        {
            var newGuest = new Guest
            {
                FullName = guest.FullName,
                Email = guest.Email,
                Phone = guest.Phone,
                IsAttending = guest.IsAttending,
                Allergies = guest.Allergies
            };
            _context.Guests.Add(newGuest);
            try
            {
                await _context.SaveChangesAsync();
                return newGuest.Id;
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                // Handle exceptions related to database updates
                throw new InvalidOperationException("A guest with the same email already exists.", ex);
            }

            static bool IsUniqueViolation(DbUpdateException ex)
            {
                // Check if the exception is due to a unique constraint violation
                return ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE constraint failed");
            }
        }

        public async Task<bool> DeleteGuestAsync(int guestId)
        {
            var rowsAffected = await _context.Guests
                .Where(g => g.Id == guestId)
                .ExecuteDeleteAsync();
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<List<Guest>> GetAllGuestAsync()
        {
          return await _context.Guests.ToListAsync();
           
        }

        public async Task<Guest?> GetByIdAsync(int guestId)
        {
            var guest = await _context.Guests
                .FirstOrDefaultAsync(g => g.Id == guestId);
            return guest;
        }

        public async Task<bool> UpdateGuestAsync(Guest guest)
        {
            _context.Guests.Update(guest);
            var result = await _context.SaveChangesAsync();
            if(result > 0)
            {
                return true;
            }
            return false;
        }
        public Task <bool> EmailExistAsync(string email) =>
        
            _context.Guests.AnyAsync(g => g.Email == email);
        
       
    }
}
