using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IReservationService
    {
        IEnumerable<Reservation> GetAllReservations();
        Reservation? GetReservationById(string id);

        void AddReservation(Reservation newReservation);

        void EditReservation(Reservation updatedReservation);

        void DeleteReservation(Reservation reservationToDelete);
    }
}
