import { type FormEvent, useEffect, useState } from 'react';
import { useRoute, useLocation } from 'wouter';
import api from '../../lib/api';
import { Button, PageShell, StatusBanner } from '../../components';
import { PagedResult } from '../../types/paged';
import { Restaurant } from '../Restaurants/types/Restaurant';
import { Reservation, ReservationInput } from './types/Reservation';

function ReservationForm() {
  const [restaurantId, setRestaurantId] = useState('');
  const [date, setDate] = useState('');
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
    const body: ReservationInput = { restaurantId, date };
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
    <PageShell>
      <h1>{id ? 'Edit Reservation' : 'Create Reservation'}</h1>
      {error && <StatusBanner variant="error">{error}</StatusBanner>}
      <form onSubmit={handleSubmit}>
        <div className="field">
          <label htmlFor="restaurantId">Restaurant</label>
          <select
            id="restaurantId"
            name="restaurantId"
            value={restaurantId}
            onChange={(e) => setRestaurantId(e.target.value)}
          >
            <option value="">Select a restaurant…</option>
            {restaurants.map((restaurant) => (
              <option key={restaurant.id} value={restaurant.id}>
                {restaurant.name}
              </option>
            ))}
          </select>
        </div>
        <div className="field">
          <label htmlFor="date">Date</label>
          <input
            id="date"
            name="date"
            type="datetime-local"
            value={date}
            onChange={(e) => setDate(e.target.value)}
          />
        </div>
        <Button type="submit" loading={submitting}>
          {id ? 'Update' : 'Create'}
        </Button>
      </form>
    </PageShell>
  );
}

export default ReservationForm;