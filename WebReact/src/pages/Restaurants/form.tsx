import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useRoute, useLocation } from 'wouter';
import { Restaurant } from './types/Restaurant';

const RestaurantForm: React.FC = () => {
  const [restaurant, setRestaurant] = useState<Restaurant>({ name: '', cuisine: '', borough: '' });
  const [id, setId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [, setLocation] = useLocation();
  const [, params] = useRoute('/restaurants/edit/:id');

  useEffect(() => {
    if (params && params.id) {
      setId(params.id);
      console.log("--")
      console.log(params.id)
      console.log("--")
      axios.get<Restaurant>(`/api/restaurants/${params.id}`)
        .then(response => setRestaurant(response.data))
        .catch(err => setError(err.message));
    }
  }, [params?.id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setRestaurant({ ...restaurant, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const request = id
      ? axios.put(`/api/restaurants/${id}`, restaurant)
      : axios.post('/api/restaurants', restaurant);

    request.then(() => {
      setLocation('/restaurants');
    }).catch(err => {
      setError(err.message);
    });
  };

  return (
    <div>
      <h1>{id ? 'Edit Restaurant' : 'Create Restaurant'}</h1>
      {error && <div>Error: {error}</div>}
      <form onSubmit={handleSubmit}>
        <div>
          <label>Name</label>
          <input type="text" name="name" value={restaurant.name} onChange={handleChange} required />
        </div>
        <div>
          <label>Cuisine</label>
          <input type="text" name="cuisine" value={restaurant.cuisine} onChange={handleChange} required />
        </div>
        <div>
          <label>Borough</label>
          <input type="text" name="borough" value={restaurant.borough} onChange={handleChange} required />
        </div>
        <button type="submit">Save</button>
      </form>
    </div>
  );
};

export default RestaurantForm;




