import { useEffect, useState } from 'react';
import { Link } from 'wouter';
import {
  Container,
  Title,
  Card,
  Table,
  Button,
  Loader,
  Alert,
  Text,
  Group,
} from '@mantine/core';
import api from '../../lib/api';
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

  if (loading) {
    return (
      <Container size="lg" py="xl">
        <Group justify="center">
          <Loader />
          <Text>Loading restaurants…</Text>
        </Group>
      </Container>
    );
  }

  if (error) {
    return (
      <Container size="lg" py="xl">
        <Alert color="red" title="Error">
          {error}
        </Alert>
      </Container>
    );
  }

  return (
    <Container size="lg" py="xl">
      <Group justify="space-between" mb="md">
        <Title order={2} c="brand.5">
          Restaurants
        </Title>
        <Button component={Link} to="/restaurants/new" variant="light" color="brand.5">
          Create New Restaurant
        </Button>
      </Group>

      {restaurants.length === 0 ? (
        <Text c="dimmed">No restaurants yet. Create one to get started.</Text>
      ) : (
        <Card shadow="sm" radius="md" padding="lg">
          <Table>
            <Table.Thead>
              <Table.Tr>
                <Table.Th>Name</Table.Th>
                <Table.Th>Cuisine</Table.Th>
                <Table.Th>Borough</Table.Th>
                <Table.Th>Actions</Table.Th>
              </Table.Tr>
            </Table.Thead>
            <Table.Tbody>
              {restaurants.map((restaurant) => (
                <Table.Tr key={restaurant.id}>
                  <Table.Td>{restaurant.name}</Table.Td>
                  <Table.Td>{restaurant.cuisine}</Table.Td>
                  <Table.Td>{restaurant.borough}</Table.Td>
                  <Table.Td>
                    <Button component={Link} to={`/restaurants/${restaurant.id}`} variant="subtle" size="sm">
                      View
                    </Button>
                  </Table.Td>
                </Table.Tr>
              ))}
            </Table.Tbody>
          </Table>
        </Card>
      )}
    </Container>
  );
}

export default RestaurantList;
