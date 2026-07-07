import { type FormEvent, useEffect, useState } from 'react';
import { useRoute, useLocation } from 'wouter';
import api from '../../lib/api';
import { Button, PageShell, StatusBanner, TextField } from '../../components';
import { Product, ProductInput } from './types/Product';

function ProductForm() {
  const [name, setName] = useState('');
  const [price, setPrice] = useState('');
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
          setPrice(String(response.data.price));
        })
        .catch((err: any) =>
          setError(err.message ?? 'Failed to load product'),
        );
    }
  }, [params]);

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    const body: ProductInput = { name, price: Number(price) };
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
    <PageShell>
      <h1>{id ? 'Edit Product' : 'Create Product'}</h1>
      {error && <StatusBanner variant="error">{error}</StatusBanner>}
      <form onSubmit={handleSubmit}>
        <TextField label="Name" name="name" value={name} onChange={setName} />
        <TextField
          label="Price"
          name="price"
          type="number"
          value={price}
          onChange={setPrice}
        />
        <Button type="submit" loading={submitting}>
          {id ? 'Update' : 'Create'}
        </Button>
      </form>
    </PageShell>
  );
}

export default ProductForm;
