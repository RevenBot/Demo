import { type FormEvent, useEffect, useState } from 'react';
import { useRoute, useLocation } from 'wouter';
import { Container, Card, Title, Select, Button, Stack, Alert } from '@mantine/core';
import { DateTimePicker } from '@mantine/dates';
import api from '../../lib/api';
import { PagedResult } from '../../types/paged';
import { Restaurant } from '../Restaurants/types/Restaurant';
import { Reservation, ReservationInput } from './types/Reservation';
import { parseUtcDate, toUtcIso } from './utils/date';

function ReservationForm() {
  const [restaurantId, setRestaurantId] = useState('');
  const [date, setDate] = useState<string>('');
  const [id, setId] = useState<string | null>(null);
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [, setLocation] = useLocation();
  const [, params] = useRoute('/reservations/edit/:id');

  useEffect(() => {
    api
      .get<PagedResult<Restaurant>>('/restaurants?pageNumber=1&pageSize=50')
      .then((response) => setRestaurants(response.data.items))
      .catch((err: any) =>
        setError(err.message ?? 'Failed to load restaurants'),
      );
  }, []);

  useEffect(() => {
    if (params && params.id) {
      setId(params.id);
      api
        .get<Reservation>(`/reservations/${params.id}`)
        .then((response) => {
          setRestaurantId(response.data.restaurantId);
          setDate(response.data.date);
        })
        .catch((err: any) =>
          setError(err.message ?? 'Failed to load reservation'),
        );
    }
  }, [params]);

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    const body: ReservationInput = {
      restaurantId,
      date,
    };
    setSubmitting(true);
    const request = id
      ? api.put(`/reservations/${id}`, body)
      : api.post('/reservations', body);

    request
      .then(() => setLocation('/reservations'))
      .catch((err: any) =>
        setError(err.message ?? 'Failed to save reservation'),
      )
      .finally(() => setSubmitting(false));
  };

  return (
    <Container size="sm" py="xl">
      <Card withBorder shadow="sm" radius="md">
        <Title order={2} mb="md">
          {id ? 'Edit Reservation' : 'Create Reservation'}
        </Title>

        {error && (
          <Alert color="red" title="Error" mb="md">
            {error}
          </Alert>
        )}

        <form onSubmit={handleSubmit}>
          <Stack>
            <Select
              label="Restaurant"
              data={restaurants.map((r) => ({ value: r.id, label: r.name }))}
              value={restaurantId}
              onChange={(val) => setRestaurantId(val ?? '')}
              placeholder="Select a restaurant…"
            />
            <DateTimePicker
              label="Date & Time"
              value={parseUtcDate(date)}
              onChange={(val) => setDate(toUtcIso(val))}
              placeholder="Pick date and time"
              clearable
              timeInputProps={{ withSeconds: false }}
            />
            <Button type="submit" loading={submitting}>
              {id ? 'Update' : 'Create'}
            </Button>
          </Stack>
        </form>
      </Card>
    </Container>
  );
}

export default ReservationForm;
