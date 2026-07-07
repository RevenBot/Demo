import { useEffect, useState } from 'react';
import { Link } from 'wouter';
import api from '../../lib/api';
import { PageShell, StatusBanner } from '../../components';
import { PagedResult } from '../../types/paged';
import { Restaurant } from './types/Restaurant';

function RestaurantList() {
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchRestaurants = async () => {
      try {
        const response = await api.get<PagedResult<Restaurant>>(
          '/restaurants?pageNumber=1&pageSize=10',
        );
        setRestaurants(response.data.items);
      } catch (err: any) {
        setError(err.message ?? 'Failed to load restaurants');
      } finally {
        setLoading(false);
      }
    };

    fetchRestaurants();
  }, []);

  return (
    <PageShell>
      <h1>Restaurant List</h1>
      <Link href="/restaurants/new">Create New Restaurant</Link>
      {loading ? (
        <StatusBanner variant="loading">Loading restaurants…</StatusBanner>
      ) : error ? (
        <StatusBanner variant="error">{error}</StatusBanner>
      ) : restaurants.length === 0 ? (
        <StatusBanner variant="empty">
          No restaurants yet. Create one to get started.
        </StatusBanner>
      ) : (
        <ul>
          {restaurants.map((restaurant) => (
            <li key={restaurant.id}>
              <Link href={`/restaurants/${restaurant.id}`}>
                {restaurant.name}
              </Link>
            </li>
          ))}
        </ul>
      )}
    </PageShell>
  );
}

export default RestaurantList;