import { type FormEvent, useEffect, useState } from 'react';
import { useRoute, useLocation } from 'wouter';
import { Container, Card, Title, TextInput, NumberInput, Button, Stack, Alert } from '@mantine/core';
import api from '../../lib/api';
import { Product, ProductInput } from './types/Product';

function ProductForm() {
  const [name, setName] = useState('');
  const [price, setPrice] = useState<number | ''>('');
  const [id, setId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [, setLocation] = useLocation();
  const [, params] = useRoute('/products/edit/:id');

  useEffect(() => {
    if (params && params.id) {
      setId(params.id);
      api
        .get<Product>(`/products/${params.id}`)
        .then((response) => {
          setName(response.data.name);
          setPrice(response.data.price);
        })
        .catch((err: any) =>
          setError(err.message ?? 'Failed to load product'),
        );
    }
  }, [params]);

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    const body: ProductInput = { name, price: price === '' ? 0 : price };
    setSubmitting(true);
    const request = id
      ? api.put(`/products/${id}`, body)
      : api.post('/products', body);

    request
      .then(() => setLocation('/products'))
      .catch((err: any) => setError(err.message ?? 'Failed to save product'))
      .finally(() => setSubmitting(false));
  };

  return (
    <Container size="sm" py="xl">
      <Card withBorder shadow="sm" radius="md">
        <Title order={2} mb="md">
          {id ? 'Edit Product' : 'Create Product'}
        </Title>

        {error && (
          <Alert color="red" title="Error" mb="md">
            {error}
          </Alert>
        )}

        <form onSubmit={handleSubmit}>
          <Stack>
            <TextInput
              label="Name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
            <NumberInput
              label="Price"
              value={price}
              onChange={(val) => setPrice(typeof val === 'number' ? val : val === '' ? ('' as const) : Number(val))}
              min={0}
              decimalScale={2}
              required
            />
            <Button type="submit" loading={submitting}>
              {id ? 'Update' : 'Create'}
            </Button>
          </Stack>
        </form>
      </Card>
    </Container>
  );
}

export default ProductForm;
