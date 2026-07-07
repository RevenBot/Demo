using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IReservationService
    {
        Task<PagedResult<Reservation>> GetAllReservationsAsync(int skip, int take);
        Task<Reservation?> GetReservationByIdAsync(string id);

        Task<Reservation> AddReservationAsync(Reservation newReservation);

        Task<Reservation> EditReservationAsync(Reservation updatedReservation);

        Task DeleteReservationAsync(Reservation reservationToDelete);
    }
}
