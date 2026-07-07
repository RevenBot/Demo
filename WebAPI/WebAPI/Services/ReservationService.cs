using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using WebAPI.Models;

namespace WebAPI.Services
{
    public class ReservationService : IReservationService
    {
        private readonly RestaurantReservationDbContext _dbContext;

        public ReservationService(RestaurantReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PagedResult<Reservation> GetAllReservations(int skip, int take)
        {
            int totalCount = _dbContext.Reservations.Count();
            var items = _dbContext.Reservations.Skip(skip).Take(take).ToList();
            
            return new PagedResult<Reservation>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = take,
                CurrentPage = (skip / take) + 1
            };
        }

        public Reservation AddReservation(Reservation reservation)
        {
            _dbContext.Reservations.Add(reservation);
            _dbContext.SaveChanges();

            return reservation;
        }

        public Reservation EditReservation(Reservation updated)
        {
            _dbContext.Reservations.Update(updated);
            _dbContext.SaveChanges();

            return updated;
        }

        public void DeleteReservation(Reservation reservation)
        {
            var reservationToDelete = _dbContext.Reservations.FirstOrDefault(b => b.Id == reservation.Id);
            
            if (reservationToDelete != null)
            {
                _dbContext.Reservations.Remove(reservationToDelete);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The reservation to delete cannot be found.");
            }
        }

        public Reservation? GetReservationById(string id)
        {
            return _dbContext.Reservations.AsNoTracking().FirstOrDefault(b => b.Id == id);
        }
    }
}
