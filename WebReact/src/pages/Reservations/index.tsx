import { useEffect, useState } from 'react';
import { Link } from 'wouter';
import api from '../../lib/api';
import { PageShell, StatusBanner } from '../../components';
import { PagedResult } from '../../types/paged';
import { Reservation } from './types/Reservation';

function ReservationList() {
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchReservations = async () => {
      try {
        const response = await api.get<PagedResult<Reservation>>(
          '/reservations?pageNumber=1&pageSize=10',
        );
        setReservations(response.data.items);
      } catch (err: any) {
        setError(err.message ?? 'Failed to load reservations');
      } finally {
        setLoading(false);
      }
    };

    fetchReservations();
  }, []);

  return (
    <PageShell>
      <h1>Reservation List</h1>
      <Link href="/reservations/new">Create New Reservation</Link>
      {loading ? (
        <StatusBanner variant="loading">Loading reservations…</StatusBanner>
      ) : error ? (
        <StatusBanner variant="error">{error}</StatusBanner>
      ) : reservations.length === 0 ? (
        <StatusBanner variant="empty">
          No reservations yet. Create one to get started.
        </StatusBanner>
      ) : (
        <ul>
          {reservations.map((reservation) => (
            <li key={reservation.id}>
              <Link href={`/reservations/${reservation.id}`}>
                {reservation.restaurantName ?? reservation.restaurantId} —{' '}
                {reservation.date}
              </Link>
            </li>
          ))}
        </ul>
      )}
    </PageShell>
  );
}

export default ReservationList;