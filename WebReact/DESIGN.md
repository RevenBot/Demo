# WebReact Design System

> **Design system**: Mantine v7 (`@mantine/core`, `@mantine/hooks`, `@mantine/dates`).
> **Theme**: defined in `src/theme.ts` using `createTheme`.
> **Provider**: `MantineProvider` wraps the app in `src/main.tsx`; default color scheme is `light`.
> **Layout**: single `AppShell` header nav in `src/App.tsx`; pages do NOT wrap themselves in layout shells.
> **Tokens**: primary brand color `#5e6ad2`, error red `#ef4444`, Inter font, radius `sm/md/lg` mapped from previous CSS variables, spacing `xs/sm/md/lg/xl/2xl`.
> **Legacy primitives removed**: `PageShell`, `Button`, `TextField`, `StatusBanner` (deleted from `src/components/`).

> Source references: Layer A = `taste-skill` (neutral operational), Layer B = `linear.app` (light-mode neutrals).
> This is a plain CRUD app (Products, Restaurants, Reservations). The surface is plain-but-intentional, Linear-style clean operational UI — not glossy.

## 1. Overview & Intent

A quiet command center for CRUD data. Dense when needed, spacious when not. The signature is **muted depth on a light canvas** — surfaces separated by subtle tonal shifts and whisper-thin borders, with a single indigo accent reserved exclusively for primary actions. No decorative gradients, no glow effects, no purple-on-white AI-slop. Every element earns its place by carrying information or an action.

The product feels like Linear's light mode: near-white background, near-black text, one brand accent (`#5e6ad2`) that appears only on primary CTAs, active nav, and focus rings. Secondary actions are ghost/subtle. Danger is a restrained red used only for destructive intent. The typeface is Inter — acceptable here because the brief explicitly calls for a neutral, Linear-style feel.

**Dials:** `DESIGN_VARIANCE: 4` (predictable grid, slight offset) · `MOTION_INTENSITY: 3` (static — CSS `:hover`/`:active`/`:focus` only, no scroll choreography) · `VISUAL_DENSITY: 5` (daily-app spacing). Light mode is the default; dark mode is out of scope for this wave.

## 2. Color Tokens

All colors are defined in `src/theme.ts` as Mantine color arrays (10-shade scales). No raw hex appears outside this file and `theme.ts`.

| Role | Mantine Token | Base Value | Usage |
|------|-------|-------|-------|
| Primary brand | `theme.colors.brand[5]` | `#5e6ad2` | Primary CTAs, active nav, focus rings |
| Brand foreground | `white` | `#ffffff` | Text/icons on brand surfaces |
| Border | `theme.colors.gray[2]` | `#e5e7eb` | Dividers, card edges |
| Danger | `theme.colors.red[5]` | `#ef4444` | Destructive buttons, error text |
| Muted text | `theme.colors.gray[5]` | `#6b7280` | Helper text, placeholders, metadata |
| Body text | `theme.colors.gray[8]` | `#1f2937` | Headlines, body text |
| Surface | `theme.colors.gray[0]` | `#f9fafb` | Card backgrounds, page canvas |

### Rules
- **One accent, locked.** `brand` is the only chromatic brand color. It never appears decoratively — only on interactive primary surfaces, active navigation, and focus rings.
- **Surface hierarchy via tonal shift.** The page background is light; elevated surfaces (cards, inputs) use `withBorder` and subtle shadows.
- **Never introduce a color not in this table.** Extend `theme.ts` first.
- **No pure `#000000` / `#ffffff` as text.** Body text uses `gray[8]`; white is acceptable only on saturated brand surfaces for ≥4.5:1 contrast.

## 3. Typography

### Font Stack
- **Sans (primary):** `'Inter', system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif` — Inter is acceptable here per the explicit Linear-style brief.
- **Mono:** `ui-monospace, 'SF Mono', 'Cascadia Code', 'Roboto Mono', Menlo, monospace` — used rarely (no code surfaces in this CRUD scope, reserved for future numeric/ID display).

### Scale

| Level | Size | Weight | Line Height | Tracking | Usage |
|-------|------|--------|-------------|----------|-------|
| Page title | 28px / 1.75rem | 600 | 1.3 | -0.01em | `<h1>` per page |
| Section heading | 20px / 1.25rem | 600 | 1.4 | 0 | `<h2>`, card titles |
| Body | 16px / 1rem | 400 | 1.5 | 0 | Default text, table cells |
| Body sm | 14px / 0.875rem | 400 | 1.5 | 0 | Secondary info, table secondary |
| Label | 14px / 0.875rem | 500 | 1.4 | 0 | Form labels, button text |
| Caption | 13px / 0.8125rem | 400 | 1.4 | 0 | Helper text, metadata, error text |

### Rules
- Max 2 font families (sans + mono). Mono is reserved; no third family.
- Body text never below 14px. Caption (13px) is reserved for helper/error/metadata only — never running body copy.
- Headings that would wrap to 4+ lines are too large; reduce the size rather than the content.
- Button text uses the Label level (14px / 500) — single line, no wrap.

## 4. Spacing & Layout

### Base Unit
All spacing derives from a base of **4px**. Every margin/padding/gap maps to a token — no magic numbers.

| Token | Value | Usage |
|-------|-------|-------|
| `theme.spacing.xs` | 4px | Tight: icon-to-label, inline group gap |
| `theme.spacing.sm` | 8px | Compact: list item gap, field internal gap |
| `theme.spacing.md` | 12px | Default: form field vertical rhythm, label-to-input |
| `theme.spacing.lg` | 16px | Standard: card padding, input horizontal padding |
| `theme.spacing.xl` | 24px | Comfortable: between form fields, card internal sections |
| `theme.spacing.2xl` | 32px | Generous: between page sections, nav-to-content |

### Radii

| Token | Value | Usage |
|-------|-------|-------|
| `theme.radius.sm` | 6px | Buttons, inputs, pills — Linear's "comfortable" radius |
| `theme.radius.md` | 8px | Cards, panels, banners |
| `theme.radius.lg` | 12px | Large containers, modals |

### Grid & Container
- Max content width: `1200px`, centered with `auto` margins.
- Nav height: `60px` desktop (AppShell header), sticky top.
- Breakpoints: `sm 640px`, `md 768px`, `lg 1024px`. Mobile-first; multi-column layouts collapse to single column below `768px`.
- Page content horizontal padding: `md` (16px) mobile, `2xl` (32px) ≥768px.

### Rules
- No magic numbers. Every spacing value is a multiple of 4px and ideally references a theme token.
- Asymmetric spacing is intentional and documented — none exists in this wave.

## 5. Mantine Components

All pages import directly from `@mantine/core` and `wouter`. No local component primitives exist.

### Components to use

| Mantine Component | Usage |
|---|---|
| `AppShell` | Single layout shell in `src/App.tsx`; header with nav, no per-page wrappers |
| `Container` | Page-level content wrapper with max-width |
| `Card` | Elevated surfaces for forms, data sections |
| `Title` | Page and section headings (`order={1-6}`) |
| `Text` | Body text, labels, metadata |
| `Button` | Actions; `variant="filled"` for primary, `"outline"` for secondary, `"light"` for ghost |
| `Group` | Horizontal flex layout with gap |
| `Stack` | Vertical flex layout with gap |
| `Table` | Data tables (Products, Restaurants, Reservations) |
| `TextInput` | Text input fields |
| `NumberInput` | Numeric input fields |
| `Select` | Dropdown selection |
| `Loader` | Loading spinners |
| `Alert` | Status messages (error, info, success) |
| `SimpleGrid` | Responsive grid layouts |
| `Anchor` | Links; use `component={Link}` with wouter `to` prop |
| `Burger` | Mobile nav toggle |

### Components NOT to use

The following legacy local primitives have been **deleted** and must NOT be recreated:
- `PageShell` — replaced by `AppShell` in `src/App.tsx`
- `Button` — replaced by Mantine `Button`
- `TextField` — replaced by Mantine `TextInput` / `NumberInput`
- `StatusBanner` — replaced by Mantine `Alert` + `Loader`

## 6. Motion Rules

**Dial: `MOTION_INTENSITY: 3` (static).** No scroll-triggered animations, no entry choreography, no marquees, no parallax. Motion serves meaning only.

### Timing

| Type | Duration | Easing | Usage |
|------|----------|--------|-------|
| Micro | 120ms | `ease-out` | Button hover/active, input focus border |
| Indeterminate | 800ms | `linear` (infinite) | Loading spinner rotation only |

### Rules
- **GPU-composited only.** Animate `transform`, `opacity`, `filter` — plus `background-color`/`border-color` for state transitions (these are cheap and do not trigger layout). NEVER animate `width`, `height`, `top`, `left`, `margin`, `padding`.
- **Every animation maps to a real state change.** Hover = affordance feedback. Active = tactile press. Focus = keyboard locator. Spinner = work in progress. No decorative motion. No hover that changes nothing.
- **`prefers-reduced-motion: reduce`** disables the spinner rotation and all transitions. The spinner still renders (static) so the loading state remains legible; only the motion stops.
- **No `will-change` spam.** Applied only to the spinner element for the duration it is mounted.
- **No `window.addEventListener('scroll')`.** Not used; not needed at this motion level.

## 7. Accessibility & Accepted Debt

### Accessibility (non-negotiable)
- **Semantic landmarks:** `AppShell` provides `<header>` and `<main>` landmarks; pages use `Container`/`Card` for content grouping.
- **Native elements:** `<button>` for actions, `<a>`/wouter `<Link>` for navigation, `<label>`+`<input>` for fields. No `<div onClick>` interactive hacks.
- **Focus rings never removed.** Mantine's default focus styles are preserved; `outline: none` is banned.
- **Keyboard operability:** all interactive primitives are operable via Tab/Enter/Space natively (no custom keyboard handlers needed).
- **ARIA:** Mantine components provide appropriate ARIA attributes out of the box.
- **Contrast:** `gray[8]` on `gray[0]` = high contrast (AAA). `brand[5]` on white = 4.6:1 (AA). `red[5]` on white = 5.9:1 (AA). All pass WCAG AA; body text passes AAA.

### Accepted Debt
- **Light mode only this wave.** Dark mode (`prefers-color-scheme: dark`) is out of scope; theme tokens are not yet dual-mode. Tracked for a later wave.
- **`gray[5]` is AA, not AAA.** Reserved for helper text, placeholders, and metadata — never running body copy. Body always uses `gray[8]` (AAA). Documented, not a violation.
- **No icon library.** Mantine's built-in spinners are used. If real icons are needed later, install `@tabler/icons-react` — do not hand-roll SVGs.
- **No Lighthouse audit run this wave.** The full Playwright + Lighthouse 100 audit runs after page composition.

## Project Rules (apply always)

- **ESLint zero-warnings:** `npm run lint` runs with `--max-warnings 0`. Any warning fails CI.
- **No unused locals/parameters:** `tsconfig.app.json` enforces `noUnusedLocals` and `noUnusedParameters`. Unused vars fail `tsc -b` and thus `npm run build`.
- **Exhaustive deps:** `useEffect` dependency arrays must use full object references, e.g. `[params]` not `[params?.id]`.
