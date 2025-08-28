using Microsoft.EntityFrameworkCore;
using WeddingInvite.Api.Models;

namespace WeddingInvite.Api.Data
{
    public class WeddingContext : DbContext
    {
        public WeddingContext(DbContextOptions<WeddingContext> options) : base(options)
        {
        }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Table>()
                .HasIndex(t => t.TableNumber)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.FK_TableId, b.StartTime });

            modelBuilder.Entity<Admin>()
                .HasData(new Admin{Id = 1,UserName = "admin",PasswordHash = "AQAAAA"});
        }
    }
}
