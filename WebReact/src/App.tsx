import './App.css'
import { Link, Route } from 'wouter';
import ProductList from './pages/Products';
import ProductForm from './pages/Products/form';
import ProductDetail from './pages/Products/detail';

function App() {
  return (
    <div>
      <nav>
        <Link href="/">Home</Link>
        <Link href="/products">Products</Link>
      </nav>

      <Route path="/" component={() => <h1>Welcome to the Home Page</h1>} />
      <Route path="/products" component={ProductList} />
      <Route path="/products/new" component={ProductForm} />
      <Route path="/products/:id" component={ProductDetail} />
      <Route path="/products/:id/edit" component={ProductForm} />
    </div>
  );
}

export default App
