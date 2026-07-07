import './App.css'
import { Link, Route } from 'wouter';
import ProductList from './pages/Products';
import ProductForm from './pages/Products/form';
import ProductDetail from './pages/Products/detail';
import RestaurantList from './pages/Restaurants';
import RestaurantForm from './pages/Restaurants/form';
import RestaurantDetail from './pages/Restaurants/detail';
import ReservationList from './pages/Reservations';
import ReservationForm from './pages/Reservations/form';
import ReservationDetail from './pages/Reservations/detail';

function App() {
  return (
    <div>
      <nav>
        <Link href="/">Home</Link>
        <Link href="/products">Products</Link>
        <Link to="/restaurants">Restaurants</Link>
        <Link to="/reservations">Reservations</Link>
      </nav>

      <Route path="/" component={() => <h1>Welcome to the Home Page</h1>} />
      <Route path="/products" component={ProductList} />
      <Route path="/products/new" component={ProductForm} />
      <Route path="/products/:id">
        {(params) => (params.id !== 'new' ? <ProductDetail /> : null)}
      </Route>
      <Route path="/products/edit/:id" component={ProductForm} />

      <Route path="/restaurants" component={RestaurantList} />
      <Route path="/restaurants/new" component={RestaurantForm} />
      <Route path="/restaurants/:id">
        {(params) => (params.id !== 'new' ? <RestaurantDetail /> : null)}
      </Route>
      <Route path="/restaurants/edit/:id" component={RestaurantForm} />

      <Route path="/reservations" component={ReservationList} />
      <Route path="/reservations/new" component={ReservationForm} />
      <Route path="/reservations/:id">
        {(params) => (params.id !== 'new' ? <ReservationDetail /> : null)}
      </Route>
      <Route path="/reservations/edit/:id" component={ReservationForm} />
    </div>
  );
}

export default App
