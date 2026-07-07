import { useEffect, useState } from 'react';
import { Link } from 'wouter';
import api from '../../lib/api';
import { PageShell, StatusBanner } from '../../components';
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
    <PageShell>
      <h1>Product List</h1>
      <Link href="/products/new">Create New Product</Link>
      {loading ? (
        <StatusBanner variant="loading">Loading products…</StatusBanner>
      ) : error ? (
        <StatusBanner variant="error">{error}</StatusBanner>
      ) : products.length === 0 ? (
        <StatusBanner variant="empty">
          No products yet. Create one to get started.
        </StatusBanner>
      ) : (
        <ul>
          {products.map((product) => (
            <li key={product.id}>
              <Link href={`/products/${product.id}`}>{product.name}</Link>
            </li>
          ))}
        </ul>
      )}
    </PageShell>
  );
}

export default ProductList;
