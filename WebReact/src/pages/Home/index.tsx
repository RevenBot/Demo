import { Link } from 'wouter';
import {
  Container,
  Title,
  Text,
  SimpleGrid,
  Card,
  Button,
} from '@mantine/core';

const RESOURCE_CARDS = [
  {
    title: 'Products',
    description: 'Manage product catalog and inventory.',
    to: '/products',
  },
  {
    title: 'Restaurants',
    description: 'Browse and manage restaurant listings.',
    to: '/restaurants',
  },
  {
    title: 'Reservations',
    description: 'View and manage table reservations.',
    to: '/reservations',
  },
];

export default function Home() {
  return (
    <Container size="lg" py="xl">
      <Title order={1} mb="xs" c="brand.5">
        Demo Dashboard
      </Title>
      <Text c="dimmed" mb="xl">
        Quick access to all resource sections.
      </Text>

      <SimpleGrid cols={{ base: 1, sm: 3 }} spacing="lg">
        {RESOURCE_CARDS.map((card) => (
          <Card key={card.to} shadow="sm" radius="md" padding="lg">
            <Title order={3} mb="xs">
              {card.title}
            </Title>
            <Text c="dimmed" mb="md" size="sm">
              {card.description}
            </Text>
            <Button component={Link} to={card.to} variant="light" color="brand.5">
              View all
            </Button>
          </Card>
        ))}
      </SimpleGrid>
    </Container>
  );
}
