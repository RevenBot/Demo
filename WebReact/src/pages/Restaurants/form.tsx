import { type FormEvent, useEffect, useState } from 'react';
import { useRoute, useLocation } from 'wouter';
import api from '../../lib/api';
import { Button, PageShell, StatusBanner, TextField } from '../../components';
import { Restaurant, RestaurantInput } from './types/Restaurant';

function RestaurantForm() {
  const [name, setName] = useState('');
  const [cuisine, setCuisine] = useState('');
  const [borough, setBorough] = useState('');
  const [id, setId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [, setLocation] = useLocation();
  const [, params] = useRoute('/restaurants/edit/:id');

  useEffect(() => {
    if (params && params.id) {
      setId(params.id);
      api
        .get<Restaurant>(`/restaurants/${params.id}`)
        .then((response) => {
          setName(response.data.name);
          setCuisine(response.data.cuisine);
          setBorough(response.data.borough);
        })
        .catch((err: any) =>
          setError(err.message ?? 'Failed to load restaurant'),
        );
    }
  }, [params]);

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    const body: RestaurantInput = { name, cuisine, borough };
    setSubmitting(true);
    const request = id
      ? api.put(`/restaurants/${id}`, body)
      : api.post('/restaurants', body);

    request
      .then(() => setLocation('/restaurants'))
      .catch((err: any) =>
        setError(err.message ?? 'Failed to save restaurant'),
      )
      .finally(() => setSubmitting(false));
  };

  return (
    <PageShell>
      <h1>{id ? 'Edit Restaurant' : 'Create Restaurant'}</h1>
      {error && <StatusBanner variant="error">{error}</StatusBanner>}
      <form onSubmit={handleSubmit}>
        <TextField label="Name" name="name" value={name} onChange={setName} />
        <TextField
          label="Cuisine"
          name="cuisine"
          value={cuisine}
          onChange={setCuisine}
        />
        <TextField
          label="Borough"
          name="borough"
          value={borough}
          onChange={setBorough}
        />
        <Button type="submit" loading={submitting}>
          {id ? 'Update' : 'Create'}
        </Button>
      </form>
    </PageShell>
  );
}

export default RestaurantForm;