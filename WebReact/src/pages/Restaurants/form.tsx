import { type FormEvent, useEffect, useState } from 'react';
import { useRoute, useLocation } from 'wouter';
import { Container, Card, Title, TextInput, Button, Stack, Alert } from '@mantine/core';
import api from '../../lib/api';
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
    <Container size="sm" py="xl">
      <Card withBorder shadow="sm" radius="md" p="xl">
        <Stack gap="md">
          <Title order={2}>{id ? 'Edit Restaurant' : 'Create Restaurant'}</Title>

          {error && (
            <Alert color="red" title="Error">
              {error}
            </Alert>
          )}

          <form onSubmit={handleSubmit}>
            <Stack gap="md">
              <TextInput
                label="Name"
                placeholder="Restaurant name"
                value={name}
                onChange={(e) => setName(e.currentTarget.value)}
              />
              <TextInput
                label="Cuisine"
                placeholder="Type of cuisine"
                value={cuisine}
                onChange={(e) => setCuisine(e.currentTarget.value)}
              />
              <TextInput
                label="Borough"
                placeholder="Borough or area"
                value={borough}
                onChange={(e) => setBorough(e.currentTarget.value)}
              />
              <Button type="submit" loading={submitting}>
                {id ? 'Update' : 'Create'}
              </Button>
            </Stack>
          </form>
        </Stack>
      </Card>
    </Container>
  );
}

export default RestaurantForm;