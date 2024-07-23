// src/ProductDetail.tsx
import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useRoute } from 'wouter';
import { Link } from 'wouter';

interface Product {
  id: number;
  name: string;
  price: number;
}

const ProductDetail: React.FC = () => {
  const [product, setProduct] = useState<Product | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [_, params] = useRoute('/products/:id');

  useEffect(() => {
    if (params && params.id) {
      axios.get<Product>(`/api/products/${params.id}`)
        .then(response => setProduct(response.data))
        .catch(err => setError(err.message));
    }
  }, [params]);

  const handleDelete = () => {
    if (product) {
      axios.delete(`/api/products/${product.id}`)
        .then(() => {
          window.location.href = '/products';
        })
        .catch(err => setError(err.message));
    }
  };

  if (!product) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div>
      <h1>Product Details</h1>
      <p>Name: {product.name}</p>
      <p>Price: ${product.price.toFixed(2)}</p>
      <Link href={`/products/${product.id}/edit`}>Edit</Link>
      <button onClick={handleDelete}>Delete</button>
    </div>
  );
};

export default ProductDetail;

