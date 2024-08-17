using MongoDB.Bson;
using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IReservationService
    {
        IEnumerable<Reservation> GetAllReservations();
        Reservation? GetReservationById(ObjectId id);

        void AddReservation(Reservation newReservation);

        void EditReservation(Reservation updatedReservation);

        void DeleteReservation(Reservation reservationToDelete);
    }
}
