# WebReact

React 18 + TypeScript SPA for the Demo monorepo.

## Stack

- **React 18** with hooks
- **TypeScript** (strict mode, `noUnusedLocals`, `noUnusedParameters`)
- **Vite 5** — dev server, build, HMR
- **Mantine v7** — UI components
- **wouter** — routing (not react-router)
- **axios** — HTTP client

## Scripts

```bash
npm install       # install dependencies
npm run dev       # Vite dev server → http://localhost:5173
npm run build     # tsc -b && vite build (typecheck fails the build)
npm run lint      # eslint --max-warnings 0 (any warning fails CI)
npm run preview   # preview production build
```

## API Proxy

In dev mode, the SPA makes HTTP requests directly to `http://localhost:5129` (the WebAPI dev server). There is no Vite proxy configured.

In the containerized deployment, `nginx` proxies `/api/` and `/swagger/` to the `myapi` service (`:8080`). The SPA is served at `http://localhost:80`.

## Resources

The app manages three resource domains via the API:

- **Products** — CRUD listing
- **Restaurants** — CRUD listing
- **Reservations** — CRUD listing

## Linting

ESLint runs with `--max-warnings 0` — **any warning fails CI**. Key rules:

- `noUnusedLocals` / `noUnusedParameters` enforced by `tsconfig.app.json`
- `exhaustive-deps` requires full object references in `useEffect` arrays (e.g. `[params]` not `[params?.id]`)
- `@typescript-eslint/no-explicit-any` is **off**

## Routing

Uses **`wouter`**, not `react-router`. Routes are defined in `src/App.tsx`.
