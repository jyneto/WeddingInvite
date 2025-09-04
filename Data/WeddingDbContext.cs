using Microsoft.EntityFrameworkCore;
using WeddingInvite.Api.Models;

namespace WeddingInvite.Api.Data
{
    public class WeddingDbContext : DbContext
    {
        public WeddingDbContext(DbContextOptions<WeddingDbContext> options) : base(options)
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

            modelBuilder.Entity<MenuItem>()
                 .Property(m => m.Price)
                 .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Guest>()
            .HasIndex(e => e.Email)
            .IsUnique();


            modelBuilder.Entity<Table>()
                .HasIndex(t => t.TableNumber)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.FK_TableId, b.StartTime });



        }
    }
}
