# WebReact Design System

This document describes the design system and UI conventions for the WebReact SPA in the Demo monorepo.

## Stack

- **React 18** with functional components and hooks
- **TypeScript** in strict mode
- **Vite 5** for dev server, build, and HMR
- **Mantine v7** as the intended UI component library
- **wouter** for lightweight routing
- **axios** for HTTP requests

## Design Principles

- Keep pages simple, consistent, and resource-oriented.
- Reuse the same list / detail / form pattern across Products, Restaurants, and Reservations.
- Show clear loading, error, and empty states.
- Prefer Mantine components for layout, forms, tables, and feedback once adopted.

## Theme

- **Font**: Inter, system-ui, Avenir, Helvetica, Arial, sans-serif.
- **Primary accent**: `#646cff`.
- **Color scheme**: dark default with a light-mode override via `prefers-color-scheme`.
- **Root layout**: max-width `1280px`, centered, with `2rem` padding.

## Layout

- A top-level navigation bar links to Home, Products, and Restaurants.
- Each page has a single heading and a content area.
- List pages include a "Create New" action above the data.

## Components

### Navigation

- Use `wouter` `Link` components for internal navigation.
- Keep the nav minimal and consistent across routes.

### Lists

- Heading + create action + data table or list.
- Display a loading spinner or message while fetching.
- Display an error message if the request fails.

### Forms

- Use labeled inputs grouped in a form.
- Use Mantine form components (`TextInput`, `NumberInput`, `Button`, etc.) when available.
- Validate required fields client side.
- Show server errors near the submit action.

### Detail Views

- Display entity fields read-only.
- Provide Edit and Delete actions.
- Confirm destructive actions before sending the delete request.

## Patterns

- Fetch data inside `useEffect` on mount.
- Read route parameters with `useRoute`.
- Navigate programmatically with `useLocation`.
- Keep state close to the page component.
- Share TypeScript types under `src/pages/<Resource>/types/`.

## API Integration

- Base API calls on `/api/<resource>`.
- In Vite dev, requests go directly to `http://localhost:5129`.
- In the containerized setup, nginx proxies `/api/` to the WebAPI service.
