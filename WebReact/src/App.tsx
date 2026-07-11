import { Link, Route, useLocation } from 'wouter';
import {
  AppShell,
  Burger,
  Group,
  Anchor,
  Stack,
  Text,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import ProductList from './pages/Products';
import ProductForm from './pages/Products/form';
import ProductDetail from './pages/Products/detail';
import RestaurantList from './pages/Restaurants';
import RestaurantForm from './pages/Restaurants/form';
import RestaurantDetail from './pages/Restaurants/detail';
import ReservationList from './pages/Reservations';
import ReservationForm from './pages/Reservations/form';
import ReservationDetail from './pages/Reservations/detail';
import Home from './pages/Home';

const NAV_ITEMS = [
  { label: 'Home', to: '/' },
  { label: 'Products', to: '/products' },
  { label: 'Restaurants', to: '/restaurants' },
  { label: 'Reservations', to: '/reservations' },
];

function NavLink({ to, label, active }: { to: string; label: string; active: boolean }) {
  return (
    <Anchor
      component={Link}
      to={to}
      variant="subtle"
      c={active ? 'brand.5' : 'dimmed'}
      fw={active ? 600 : 400}
      underline="never"
      style={{
        transition: 'color 150ms ease',
        ...(active ? { borderBottom: '2px solid var(--mantine-color-brand-5)' } : {}),
      }}
    >
      {label}
    </Anchor>
  );
}

function App() {
  const [location] = useLocation();
  const [opened, { toggle, close }] = useDisclosure();

  const isActive = (to: string) => {
    if (to === '/') return location === '/';
    return location === to || location.startsWith(`${to}/`);
  };

  return (
    <AppShell
      header={{ height: 60 }}
      padding="md"
    >
    <AppShell.Header>
        <Group h="100%" px="md" justify="space-between">
          <Group gap="md">
            <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" />
            <Text fw={700} size="xl" c="brand.5">
              Demo
            </Text>
          </Group>

          <Group gap="md" visibleFrom="sm">
            {NAV_ITEMS.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                label={item.label}
                active={isActive(item.to)}
              />
            ))}
          </Group>
        </Group>
      </AppShell.Header>

      {opened && (
        <Stack
          gap="xs"
          p="md"
          style={{
            position: 'fixed',
            top: 60,
            left: 0,
            right: 0,
            zIndex: 100,
            backgroundColor: 'var(--mantine-color-body)',
            borderBottom: '1px solid var(--mantine-color-gray-2)',
          }}
        >
          {NAV_ITEMS.map((item) => (
            <Anchor
              key={item.to}
              component={Link}
              to={item.to}
              onClick={close}
              variant="subtle"
              c={isActive(item.to) ? 'brand.5' : 'dimmed'}
              fw={isActive(item.to) ? 600 : 400}
              underline="never"
            >
              {item.label}
            </Anchor>
          ))}
        </Stack>
      )}

      <AppShell.Main>
        <Route path="/" component={Home} />
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
      </AppShell.Main>
    </AppShell>
  );
}

export default App;
