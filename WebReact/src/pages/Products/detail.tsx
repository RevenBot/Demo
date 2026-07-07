import { useEffect, useState } from 'react';
import { Link, useRoute, useLocation } from 'wouter';
import api from '../../lib/api';
import { Button, PageShell, StatusBanner } from '../../components';
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

  return (
    <PageShell>
      <h1>Product Details</h1>
      {error ? (
        <StatusBanner variant="error">{error}</StatusBanner>
      ) : !product ? (
        <StatusBanner variant="loading">Loading product…</StatusBanner>
      ) : (
        <>
          <p>Name: {product.name}</p>
          <p>Price: ${product.price.toFixed(2)}</p>
          <Link href={`/products/edit/${product.id}`}>Edit</Link>
          <Button variant="danger" loading={deleting} onClick={handleDelete}>
            Delete
          </Button>
        </>
      )}
    </PageShell>
  );
}

export default ProductDetail;
