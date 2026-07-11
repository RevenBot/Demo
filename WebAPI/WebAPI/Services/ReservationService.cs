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

        public async Task<PagedResult<Reservation>> GetAllReservationsAsync(int skip, int take)
        {
            int totalCount = await _dbContext.Reservations.CountAsync();
            var items = await _dbContext.Reservations.Skip(skip).Take(take).ToListAsync();
            
            return new PagedResult<Reservation>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = take,
                CurrentPage = (skip / take) + 1
            };
        }

        public async Task<Reservation?> GetReservationByIdAsync(string id)
        {
            return await _dbContext.Reservations.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Reservation> AddReservationAsync(Reservation reservation)
        {
            await _dbContext.Reservations.AddAsync(reservation);
            await _dbContext.SaveChangesAsync();

            return reservation;
        }

        public async Task<Reservation> EditReservationAsync(Reservation updated)
        {
            _dbContext.Reservations.Update(updated);
            await _dbContext.SaveChangesAsync();

            return updated;
        }

        public async Task DeleteReservationAsync(Reservation reservation)
        {
            var reservationToDelete = await _dbContext.Reservations.FirstOrDefaultAsync(b => b.Id == reservation.Id);
            
            if (reservationToDelete != null)
            {
                _dbContext.Reservations.Remove(reservationToDelete);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("The reservation to delete cannot be found.");
            }
        }
    }
}
