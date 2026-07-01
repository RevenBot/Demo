using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using WebAPI.Models;

namespace WebAPI.Services
{
    public class RestaurantReservationDbContext : DbContext
    {
        public DbSet<Restaurant> Restaurants { get; init; }
        public DbSet<Reservation> Reservations { get; init; }

        public RestaurantReservationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure all string Ids to serialize as native MongoDB ObjectIds in the database
            // and appear as hex strings in JSON responses (not complex objects)
            modelBuilder.Entity<Restaurant>().Property(r => r.Id).IsRequired();
            modelBuilder.Entity<Reservation>().Property(r => r.Id).IsRequired();
        }
    }
}
