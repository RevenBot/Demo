import { useEffect, useState } from 'react';
import { Link, useRoute, useLocation } from 'wouter';
import api from '../../lib/api';
import { Container, Card, Title, Text, Button, Group, Loader, Alert } from '@mantine/core';
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

  if (!restaurant && !error) {
    return (
      <Container size="md" py="xl">
        <Loader color="brand.5" />
      </Container>
    );
  }

  if (error) {
    return (
      <Container size="md" py="xl">
        <Alert color="red" title="Error">
          {error}
        </Alert>
      </Container>
    );
  }

  return (
    <Container size="md" py="xl">
      <Card shadow="sm" radius="md" padding="lg">
        <Title order={2}>Restaurant Details</Title>
        <Text mt="md"><strong>Name:</strong> {restaurant!.name}</Text>
        <Text><strong>Cuisine:</strong> {restaurant!.cuisine}</Text>
        <Text><strong>Borough:</strong> {restaurant!.borough}</Text>
        <Group mt="lg">
          <Button component={Link} to={`/restaurants/edit/${restaurant!.id}`} variant="light">
            Edit
          </Button>
          <Button color="red" loading={deleting} onClick={handleDelete}>
            Delete
          </Button>
        </Group>
      </Card>
    </Container>
  );
}

export default RestaurantDetail;
