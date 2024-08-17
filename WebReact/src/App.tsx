import './App.css'
import { Link, Route } from 'wouter';
import ProductList from './pages/Products';
import ProductForm from './pages/Products/form';
import ProductDetail from './pages/Products/detail';
import RestaurantList from './pages/Restaurants';
import RestaurantForm from './pages/Restaurants/form';
import RestaurantDetail from './pages/Restaurants/detail';

function App() {
  return (
    <div>
      <nav>
        <Link href="/">Home</Link>
        <Link href="/products">Products</Link>
        <Link to="/restaurants">Restaurants</Link>
      </nav>

      <Route path="/" component={() => <h1>Welcome to the Home Page</h1>} />
      <Route path="/products" component={ProductList} />
      <Route path="/products/new" component={ProductForm} />
      <Route path="/products/:id" component={ProductDetail} />
      <Route path="/products/edit/:id" component={ProductForm} />

      <Route path="/restaurants" component={RestaurantList} />
      <Route path="/restaurants/new" component={RestaurantForm} />
      <Route path="/restaurants/:id" component={RestaurantDetail} />
      <Route path="/restaurants/edit/:id" component={RestaurantForm} />
    </div>
  );
}

export default App
