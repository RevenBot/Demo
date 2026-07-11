import { useEffect, useState } from 'react';
import { Link } from 'wouter';
import api from '../../lib/api';
import { PagedResult } from '../../types/paged';
import { Reservation } from './types/Reservation';
import { formatLocalDateTime } from './utils/date';
import {
  Container,
  Title,
  Card,
  Table,
  Button,
  Loader,
  Alert,
  Text,
  Group,
} from '@mantine/core';

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
    <Container size="lg" py="xl">
      <Group justify="space-between" mb="md">
        <Title order={2} c="brand.5">
          Reservations
        </Title>
        <Button component={Link} to="/reservations/new" variant="light" color="brand.5">
          Create New Reservation
        </Button>
      </Group>

      {loading ? (
        <Group justify="center" py="xl">
          <Loader color="brand.5" />
        </Group>
      ) : error ? (
        <Alert color="red" title="Error" mb="md">
          {error}
        </Alert>
      ) : reservations.length === 0 ? (
        <Text c="dimmed" ta="center" py="xl">
          No reservations yet. Create one to get started.
        </Text>
      ) : (
        <Card shadow="sm" radius="md" padding="md">
          <Table>
            <Table.Thead>
              <Table.Tr>
                <Table.Th>Restaurant</Table.Th>
                <Table.Th>Date</Table.Th>
                <Table.Th>Actions</Table.Th>
              </Table.Tr>
            </Table.Thead>
            <Table.Tbody>
              {reservations.map((reservation) => (
                <Table.Tr key={reservation.id}>
                  <Table.Td>
                    {reservation.restaurantName ?? reservation.restaurantId}
                  </Table.Td>
                  <Table.Td>{formatLocalDateTime(reservation.date)}</Table.Td>
                  <Table.Td>
                    <Button component={Link} to={`/reservations/${reservation.id}`} variant="subtle" size="xs">
                      View
                    </Button>
                  </Table.Td>
                </Table.Tr>
              ))}
            </Table.Tbody>
          </Table>
        </Card>
      )}
    </Container>
  );
}

export default ReservationList;
