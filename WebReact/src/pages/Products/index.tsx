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
import { Product } from './types/Product';

function ProductList() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await api.get<PagedResult<Product>>(
          '/products?pageNumber=1&pageSize=10',
        );
        setProducts(response.data.items);
      } catch (err: any) {
        setError(err.message ?? 'Failed to load products');
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);

  return (
    <Container size="lg" py="xl">
      <Group justify="space-between" mb="lg">
        <Title order={1} c="brand.5">
          Products
        </Title>
        <Button component={Link} to="/products/new" color="brand.5">
          Create New Product
        </Button>
      </Group>

      <Card shadow="sm" radius="md" padding="lg">
        {loading ? (
          <Group justify="center" py="xl">
            <Loader />
          </Group>
        ) : error ? (
          <Alert color="red" title="Error" mb="md">
            {error}
          </Alert>
        ) : products.length === 0 ? (
          <Text c="dimmed" ta="center" py="xl">
            No products yet. Create one to get started.
          </Text>
        ) : (
          <Table>
            <Table.Thead>
              <Table.Tr>
                <Table.Th>Name</Table.Th>
                <Table.Th>Price</Table.Th>
                <Table.Th>Actions</Table.Th>
              </Table.Tr>
            </Table.Thead>
            <Table.Tbody>
              {products.map((product) => (
                <Table.Tr key={product.id}>
                  <Table.Td>{product.name}</Table.Td>
                  <Table.Td>${product.price.toFixed(2)}</Table.Td>
                  <Table.Td>
                    <Button
                      component={Link}
                      to={`/products/${product.id}`}
                      variant="light"
                      size="xs"
                    >
                      View
                    </Button>
                  </Table.Td>
                </Table.Tr>
              ))}
            </Table.Tbody>
          </Table>
        )}
      </Card>
    </Container>
  );
}

export default ProductList;
