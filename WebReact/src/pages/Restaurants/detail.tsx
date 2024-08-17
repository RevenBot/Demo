import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useRoute } from 'wouter';
import { Link } from 'wouter';
import { Restaurant } from './types/Restaurant';

const RestaurantDetail: React.FC = () => {
  const [restaurant, setRestaurant] = useState<Restaurant | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [, params] = useRoute('/restaurants/:id');

  useEffect(() => {
    console.log(params)
    if (params && params.id) {
      axios.get<Restaurant>(`/api/restaurants/${params.id}`)
        .then(response => {
          setRestaurant(response.data);
          console.log(response)
        })
        .catch(err => setError(err.message));
    }
  }, [params?.id]);

  const handleDelete = () => {
    if (restaurant) {
      axios.delete(`/api/restaurants/${restaurant._id}`)
        .then(() => {
          window.location.href = '/restaurants';
        })
        .catch(err => setError(err.message));
    }
  };

  if (!restaurant) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div>
      <h1>Restaurant Details</h1>
      <p>Name: {restaurant.name}</p>
      <p>Cuisine: {restaurant.cuisine}</p>
      <p>Borough: {restaurant.borough}</p>
      <Link href={`/restaurants/edit/${restaurant._id}`}>Edit</Link>
      <button onClick={handleDelete}>Delete</button>
    </div>
  );
};

export default RestaurantDetail;

