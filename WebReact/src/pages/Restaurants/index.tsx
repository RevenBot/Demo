import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Link } from 'wouter';
import { Restaurant } from './types/Restaurant';


const RestaurantList: React.FC = () => {
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchRestaurants = async () => {
      try {
        const response = await axios.get<Restaurant[]>('/api/restaurants');
        setRestaurants(response.data);
      } catch (err: any) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchRestaurants();
  }, []);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div>
      <h1>Restaurant List</h1>
      <Link href="/restaurants/new">Create New Restaurant</Link>
      <ul>
        {restaurants.map((restaurant) => (
          <li key={restaurant._id}>
            <Link href={`/restaurants/${restaurant._id}`}>{restaurant.name}</Link>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default RestaurantList;

