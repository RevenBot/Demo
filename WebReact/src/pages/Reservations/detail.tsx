import { useEffect, useState } from 'react';
import { Link, useRoute, useLocation } from 'wouter';
import api from '../../lib/api';
import { Button, PageShell, StatusBanner } from '../../components';
import { Reservation } from './types/Reservation';

function ReservationDetail() {
  const [reservation, setReservation] = useState<Reservation | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [, setLocation] = useLocation();
  const [, params] = useRoute('/reservations/:id');

  useEffect(() => {
    if (params && params.id) {
      api
        .get<Reservation>(`/reservations/${params.id}`)
        .then((response) => setReservation(response.data))
        .catch((err: any) =>
          setError(err.message ?? 'Failed to load reservation'),
        );
    }
  }, [params]);

  const handleDelete = () => {
    if (!reservation) return;
    setDeleting(true);
    api
      .delete(`/reservations/${reservation.id}`)
      .then(() => setLocation('/reservations'))
      .catch((err: any) => {
        setError(err.message ?? 'Failed to delete reservation');
        setDeleting(false);
      });
  };

  return (
    <PageShell>
      <h1>Reservation Details</h1>
      {error ? (
        <StatusBanner variant="error">{error}</StatusBanner>
      ) : !reservation ? (
        <StatusBanner variant="loading">Loading reservation…</StatusBanner>
      ) : (
        <>
          <p>Restaurant: {reservation.restaurantName ?? reservation.restaurantId}</p>
          <p>Date: {reservation.date}</p>
          <Link href={`/reservations/edit/${reservation.id}`}>Edit</Link>
          <Button variant="danger" loading={deleting} onClick={handleDelete}>
            Delete
          </Button>
        </>
      )}
    </PageShell>
  );
}

export default ReservationDetail;