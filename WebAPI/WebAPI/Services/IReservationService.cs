using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IReservationService
    {
        PagedResult<Reservation> GetAllReservations(int skip, int take);
        Reservation? GetReservationById(string id);

        Reservation AddReservation(Reservation newReservation);

        Reservation EditReservation(Reservation updatedReservation);

        void DeleteReservation(Reservation reservationToDelete);
    }
}
