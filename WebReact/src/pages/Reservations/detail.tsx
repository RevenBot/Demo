import { useEffect, useState } from 'react';
import { Link, useRoute, useLocation } from 'wouter';
import { Container, Card, Title, Text, Button, Group, Loader, Alert } from '@mantine/core';
import api from '../../lib/api';
import { Reservation } from './types/Reservation';
import { formatLocalDateTime } from './utils/date';

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

  if (error) {
    return (
      <Container size="md" py="xl">
        <Alert color="red" title="Error">
          {error}
        </Alert>
      </Container>
    );
  }

  if (!reservation) {
    return (
      <Container size="md" py="xl" style={{ display: 'flex', justifyContent: 'center' }}>
        <Loader color="brand.5" />
      </Container>
    );
  }

  return (
    <Container size="md" py="xl">
      <Card shadow="sm" radius="md" padding="lg">
        <Title order={2}>Reservation Details</Title>
        <Text mt="md">Restaurant: {reservation.restaurantName ?? reservation.restaurantId}</Text>
        <Text>Date: {formatLocalDateTime(reservation.date)}</Text>
        <Group mt="md">
          <Button component={Link} to={`/reservations/edit/${reservation.id}`} variant="light">
            Edit
          </Button>
          <Button color="red" loading={deleting} onClick={handleDelete}>
            Delete
          </Button>
        </Group>
      </Card>
    </Container>
  );
}

export default ReservationDetail;