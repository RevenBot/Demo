import { useEffect, useState } from 'react';
import { Link, useRoute, useLocation } from 'wouter';
import { Container, Card, Title, Text, Button, Group, Loader, Alert } from '@mantine/core';
import api from '../../lib/api';
import { Product } from './types/Product';

function ProductDetail() {
  const [product, setProduct] = useState<Product | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [, setLocation] = useLocation();
  const [, params] = useRoute('/products/:id');

  useEffect(() => {
    if (params && params.id) {
      api
        .get<Product>(`/products/${params.id}`)
        .then((response) => setProduct(response.data))
        .catch((err: any) =>
          setError(err.message ?? 'Failed to load product'),
        );
    }
  }, [params]);

  const handleDelete = () => {
    if (!product) return;
    setDeleting(true);
    api
      .delete(`/products/${product.id}`)
      .then(() => setLocation('/products'))
      .catch((err: any) => {
        setError(err.message ?? 'Failed to delete product');
        setDeleting(false);
      });
  };

  if (error) {
    return (
      <Container size="md" py="xl">
        <Alert color="red" title="Error">
          {error}
        </Alert>
      </Container>
    );
  }

  if (!product) {
    return (
      <Container size="md" py="xl" style={{ display: 'flex', justifyContent: 'center' }}>
        <Loader color="brand.5" />
      </Container>
    );
  }

  return (
    <Container size="md" py="xl">
      <Card shadow="sm" radius="md" padding="lg">
        <Title order={2}>Product Details</Title>
        <Text mt="sm"><strong>Name:</strong> {product.name}</Text>
        <Text><strong>Price:</strong> ${product.price.toFixed(2)}</Text>
        <Group mt="md">
          <Button component={Link} to={`/products/edit/${product.id}`} variant="light">
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

export default ProductDetail;
