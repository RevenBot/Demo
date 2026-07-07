import { useEffect, useState } from 'react';
import { Link, useRoute, useLocation } from 'wouter';
import api from '../../lib/api';
import { Button, PageShell, StatusBanner } from '../../components';
import { Restaurant } from './types/Restaurant';

function RestaurantDetail() {
  const [restaurant, setRestaurant] = useState<Restaurant | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [, setLocation] = useLocation();
  const [, params] = useRoute('/restaurants/:id');

  useEffect(() => {
    if (params && params.id) {
      api
        .get<Restaurant>(`/restaurants/${params.id}`)
        .then((response) => setRestaurant(response.data))
        .catch((err: any) =>
          setError(err.message ?? 'Failed to load restaurant'),
        );
    }
  }, [params]);

  const handleDelete = () => {
    if (!restaurant) return;
    setDeleting(true);
    api
      .delete(`/restaurants/${restaurant.id}`)
      .then(() => setLocation('/restaurants'))
      .catch((err: any) => {
        setError(err.message ?? 'Failed to delete restaurant');
        setDeleting(false);
      });
  };

  return (
    <PageShell>
      <h1>Restaurant Details</h1>
      {error ? (
        <StatusBanner variant="error">{error}</StatusBanner>
      ) : !restaurant ? (
        <StatusBanner variant="loading">Loading restaurant…</StatusBanner>
      ) : (
        <>
          <p>Name: {restaurant.name}</p>
          <p>Cuisine: {restaurant.cuisine}</p>
          <p>Borough: {restaurant.borough}</p>
          <Link href={`/restaurants/edit/${restaurant.id}`}>Edit</Link>
          <Button variant="danger" loading={deleting} onClick={handleDelete}>
            Delete
          </Button>
        </>
      )}
    </PageShell>
  );
}

export default RestaurantDetail;