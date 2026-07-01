using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using WebAPI.Models;

namespace WebAPI.Services
{
    public class ReservationService : IReservationService
    {
        private readonly RestaurantReservationDbContext _restaurantDbContext;

        public ReservationService(RestaurantReservationDbContext restaurantDbContext)
        {
            _restaurantDbContext = restaurantDbContext;
        }

        public void AddReservation(Reservation newReservation)
        {
            // Find the referenced restaurant and set its name in the reservation
            var bookedRestaurant = _restaurantDbContext.Restaurants.FirstOrDefault(c => c.Id == newReservation.RestaurantId);
            
            if (bookedRestaurant != null)
            {
                newReservation.RestaurantName = bookedRestaurant.Name;
            }

            _restaurantDbContext.Reservations.Add(newReservation);
            _restaurantDbContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_restaurantDbContext.ChangeTracker.DebugView.LongView);
            _restaurantDbContext.SaveChanges();
        }

        public void DeleteReservation(Reservation reservation)
        {
            var reservationToDelete = _restaurantDbContext.Reservations.FirstOrDefault(b => b.Id == reservation.Id);
            
            if (reservationToDelete != null)
            {
                _restaurantDbContext.Reservations.Remove(reservationToDelete);
                _restaurantDbContext.ChangeTracker.DetectChanges();
                Console.WriteLine(_restaurantDbContext.ChangeTracker.DebugView.LongView);
                _restaurantDbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The reservation to delete cannot be found.");
            }
        }

        public void EditReservation(Reservation updated)
        {
            var existing = _restaurantDbContext.Reservations.FirstOrDefault(b => b.Id == updated.Id);
            
            if (existing != null)
            {
                // Only update fields that were actually provided in the body
                if (!string.IsNullOrEmpty(updated.RestaurantName))
                    existing.RestaurantName = updated.RestaurantName;
                if (updated.Date != default)
                    existing.Date = updated.Date;

                _restaurantDbContext.Reservations.Update(existing);
                _restaurantDbContext.ChangeTracker.DetectChanges();
                Console.WriteLine(_restaurantDbContext.ChangeTracker.DebugView.LongView);
                _restaurantDbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The reservation to update cannot be found.");
            }
        }

        public IEnumerable<Reservation> GetAllReservations()
        {
            return _restaurantDbContext.Reservations.OrderBy(b => b.Date).Take(20).AsNoTracking().ToList();
        }

        // Now accepts string ID directly - no ObjectId conversion needed
        public Reservation? GetReservationById(string id)
        {
            return _restaurantDbContext.Reservations.AsNoTracking().FirstOrDefault(b => b.Id == id);
        }
    }
}
