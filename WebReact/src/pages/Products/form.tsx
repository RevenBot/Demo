// src/ProductForm.tsx
import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useRoute, useLocation } from 'wouter';
import { Product } from './types/Product';

const ProductForm: React.FC = () => {
  const [product, setProduct] = useState<Product>({ name: '', price: 0 });
  const [id, setId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [, setLocation] = useLocation();
  const [, params] = useRoute('/products/edit/:id');

  useEffect(() => {
    if (params && params.id) {
      setId(Number(params.id));
      axios.get<Product>(`/api/products/${params.id}`)
        .then(response => setProduct(response.data))
        .catch(err => setError(err.message));
    }
  }, [params?.id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setProduct({ ...product, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const request = id
      ? axios.put(`/api/products/${id}`, product)
      : axios.post('/api/products', product);

    request.then(() => {
      setLocation('/products');
    }).catch(err => {
      setError(err.message);
    });
  };

  return (
    <div>
      <h1>{id ? 'Edit Product' : 'Create Product'}</h1>
      {error && <div>Error: {error}</div>}
      <form onSubmit={handleSubmit}>
        <div>
          <label>Name</label>
          <input type="text" name="name" value={product.name} onChange={handleChange} required />
        </div>
        <div>
          <label>Price</label>
          <input type="number" name="price" value={product.price} onChange={handleChange} required />
        </div>
        <button type="submit">Save</button>
      </form>
    </div>
  );
};

export default ProductForm;



