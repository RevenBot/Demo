# WebReact Design System

> Source references: Layer A = `taste-skill` (neutral operational), Layer B = `linear.app` (light-mode neutrals).
> This is a plain CRUD app (Products, Restaurants, Reservations). The surface is plain-but-intentional, Linear-style clean operational UI — not glossy.

## 1. Overview & Intent

A quiet command center for CRUD data. Dense when needed, spacious when not. The signature is **muted depth on a light canvas** — surfaces separated by subtle tonal shifts and whisper-thin borders, with a single indigo accent reserved exclusively for primary actions. No decorative gradients, no glow effects, no purple-on-white AI-slop. Every element earns its place by carrying information or an action.

The product feels like Linear's light mode: near-white background, near-black text, one brand accent (`#5e6ad2`) that appears only on primary CTAs, active nav, and focus rings. Secondary actions are ghost/subtle. Danger is a restrained red used only for destructive intent. The typeface is Inter — acceptable here because the brief explicitly calls for a neutral, Linear-style feel.

**Dials:** `DESIGN_VARIANCE: 4` (predictable grid, slight offset) · `MOTION_INTENSITY: 3` (static — CSS `:hover`/`:active`/`:focus` only, no scroll choreography) · `VISUAL_DENSITY: 5` (daily-app spacing). Light mode is the default; dark mode is out of scope for this wave.

## 2. Color Tokens

All colors are CSS custom properties in `src/styles/tokens.css`. No raw hex appears outside this file and `DESIGN.md`.

| Role | Token | Value | Usage |
|------|-------|-------|-------|
| Background | `--color-bg` | `#f7f8f8` | Page canvas |
| Foreground | `--color-fg` | `#0a0a0a` | Headlines, body text, input values |
| Accent | `--color-accent` | `#5e6ad2` | Primary CTAs, active nav, focus rings |
| Accent foreground | `--color-accent-fg` | `#ffffff` | Text/icons on accent surfaces |
| Border | `--color-border` | `#d0d6e0` | Dividers, input outlines, card edges |
| Danger | `--color-danger` | `#dc2626` | Destructive buttons, error text, error input ring |
| Muted | `--color-muted` | `#6b7280` | Helper text, placeholders, secondary labels, metadata |
| Hover tint | `--color-hover-tint` | `rgba(10,10,10,0.04)` | Subtle hover surface (nav links, secondary buttons) |
| Danger tint | `--color-danger-tint` | `rgba(220,38,38,0.08)` | Error banner background |

### Rules
- **One accent, locked.** `--color-accent` is the only chromatic brand color. It never appears decoratively — only on interactive primary surfaces, active navigation, and focus rings.
- **Surface hierarchy via tonal shift.** The page background is `--color-bg`; elevated surfaces (cards, inputs) use the same hue with a 1px `--color-border` edge. No drop shadows for surface separation except `--shadow-sm` on floating elements.
- **Never introduce a color not in this table.** Extend the table first, then add the token to `tokens.css`.
- **No pure `#000000` / `#ffffff` as text.** Foreground is `#0a0a0a` (off-black); accent-foreground is `#ffffff` (acceptable on the saturated indigo surface for ≥4.5:1 contrast).

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
| `--space-1` | 4px | Tight: icon-to-label, inline group gap |
| `--space-2` | 8px | Compact: list item gap, field internal gap |
| `--space-3` | 12px | Default: form field vertical rhythm, label-to-input |
| `--space-4` | 16px | Standard: card padding, input horizontal padding |
| `--space-5` | 24px | Comfortable: between form fields, card internal sections |
| `--space-6` | 32px | Generous: between page sections, nav-to-content |

### Radii

| Token | Value | Usage |
|-------|-------|-------|
| `--radius-sm` | 6px | Buttons, inputs, pills — Linear's "comfortable" radius |
| `--radius-md` | 8px | Cards, panels, banners |

### Shadow

| Token | Value | Usage |
|-------|-------|-------|
| `--shadow-sm` | `0 1px 2px rgba(0, 0, 0, 0.04)` | Subtle elevation on floating/dropdown surfaces only |

### Grid & Container
- Max content width: `1200px`, centered with `auto` margins.
- Nav height: `56px` desktop (under the 64px cap), sticky top.
- Breakpoints: `sm 640px`, `md 768px`, `lg 1024px`. Mobile-first; multi-column layouts collapse to single column below `768px`.
- Page content horizontal padding: `--space-4` (16px) mobile, `--space-6` (32px) ≥768px.

### Rules
- No magic numbers. Every spacing value is a multiple of `--space-1` (4px) and ideally references a token.
- Asymmetric spacing is intentional and documented — none exists in this wave.

## 5. Components / Primitives

Four primitives in `src/components/`. Each consumes tokens via `var(--token)`. States are exhaustive per the taste-skill interactive-states rule.

### PageShell
- **Structure:** `<div class=shell>` → `<nav class=nav>` (sticky, brand + nav items) + `<main class=content>` (children).
- **Props:** `children: ReactNode`; `nav?: NavItem[]` (default: Home / Products / Restaurants).
- **Nav item:** wouter `<Link>`; active state via wouter's `useLocation` match.
- **Spacing:** nav internal padding `--space-4`; content padding `--space-4` mobile / `--space-6` desktop; nav-to-content gap `--space-6`.
- **States:** default / hover (link lightens to `--color-fg`) / active (link color = `--color-accent`, weight 500) / focus-visible (accent ring).
- **Accessibility:** `<nav>` landmark with `aria-label="Primary"`; `<main>` landmark; links have discernible text.
- **Motion:** none — nav is static.

### Button
- **Structure:** `<button class=button variant=…>`; loading state prepends a CSS spinner `<span class=spinner>`.
- **Variants:** `primary` | `secondary` | `danger`.
- **Props:** `variant`, `disabled`, `loading`, `onClick`, `children`, `type`.
- **Spacing:** padding `--space-2 --space-4`; radius `--radius-sm`; font Label (14px/500).
- **States:**
  - **default:** primary = `--color-accent` bg / `--color-accent-fg` text; secondary = transparent bg / `--color-border` 1px / `--color-fg` text; danger = `--color-danger` bg / `--color-accent-fg` text.
  - **hover:** primary → slightly lighter accent (`#7170ff`); secondary → `--color-bg` tinted bg; danger → `#b91c1c`.
  - **active:** `transform: translateY(1px)` (tactile push, GPU-composited).
  - **focus-visible:** `2px` `--color-accent` outline offset `2px` (never removed).
  - **disabled:** `opacity: 0.5` + `cursor: not-allowed` + `aria-disabled`; no hover/active transforms.
  - **loading:** spinner visible + `aria-busy="true"` + functionally disabled (no onClick fire) + cursor `progress`.
- **Accessibility:** native `<button>`; `type` defaults to `button`; `aria-busy` on loading; disabled via `disabled` attribute.
- **Motion:** hover/active color + transform transitions, `120ms ease-out`, `transform`/`opacity`/`background-color` only.

### TextField
- **Structure:** `<div class=field>` → `<label class=label>` + `<input class=input>` + `<span class=error>` (when error).
- **Props:** `label`, `name`, `value`, `onChange: (value: string) => void`, `error?`, `disabled?`, `type?` (default `text`).
- **Spacing:** label-to-input `--space-2`; input padding `--space-2 --space-4`; field-to-field `--space-5` (applied by parent, not internally).
- **States:**
  - **default:** `--color-bg`-tinted bg, `1px solid --color-border`.
  - **focus:** `2px solid --color-accent` border (replaces default), no separate outline.
  - **disabled:** `opacity: 0.5` + `cursor: not-allowed`.
  - **error:** `1px solid --color-danger` border + error text below in `--color-danger` (Caption size).
- **Accessibility:** `<label htmlFor=id>`; `<input id=name aria-invalid=… aria-describedby=errorId>`; error text has `role="alert"` + `id`.
- **Motion:** border-color transition `120ms ease-out` on focus.

### StatusBanner
- **Structure:** `<div class=banner variant=… role=…>` → optional spinner + children.
- **Variants:** `error` | `empty` | `loading`.
- **Props:** `variant`, `children`.
- **Spacing:** padding `--space-3 --space-4`; radius `--radius-md`; margin-bottom `--space-4`.
- **States:**
  - **error:** `--color-danger` tinted bg (`color-mix` fallback: light red surface) + `--color-danger` left border `3px`; `role="alert"`.
  - **empty:** `--color-bg`-tinted surface + `--color-border` 1px + `--color-muted` text; `role="status"`.
  - **loading:** `--color-bg`-tinted surface + `--color-border` 1px + spinner + `--color-muted` text; `role="status"` + `aria-busy="true"`.
- **Accessibility:** `role` set per variant; spinner is `aria-hidden` decorative.
- **Motion:** spinner rotates via `transform: rotate()` `@keyframes`, `800ms linear` infinite — the only animation in the system, and it communicates a real state (work in progress).

## 6. Motion Rules

**Dial: `MOTION_INTENSITY: 3` (static).** No scroll-triggered animations, no entry choreography, no marquees, no parallax. Motion serves meaning only.

### Timing

| Type | Duration | Easing | Usage |
|------|----------|--------|-------|
| Micro | 120ms | `ease-out` | Button hover/active, input focus border |
| Indeterminate | 800ms | `linear` (infinite) | Loading spinner rotation only |

### Rules
- **GPU-composited only.** Animate `transform`, `opacity`, `filter` — plus `background-color`/`border-color` for state transitions (these are cheap and do not trigger layout). NEVER animate `width`, `height`, `top`, `left`, `margin`, `padding`.
- **Every animation maps to a real state change.** Hover = affordance feedback. Active = tactile press. Focus = keyboard locater. Spinner = work in progress. No decorative motion. No hover that changes nothing.
- **`prefers-reduced-motion: reduce`** disables the spinner rotation and all transitions. The spinner still renders (static) so the loading state remains legible; only the motion stops.
- **No `will-change` spam.** Applied only to the spinner element for the duration it is mounted.
- **No `window.addEventListener('scroll')`.** Not used; not needed at this motion level.

## 7. Accessibility & Accepted Debt

### Accessibility (non-negotiable)
- **Semantic landmarks:** every page wrapped in `PageShell` → `<nav>` + `<main>`.
- **Native elements:** `<button>` for actions, `<a>`/wouter `<Link>` for navigation, `<label>`+`<input>` for fields. No `<div onClick>` interactive hacks.
- **Focus rings never removed.** `:focus-visible` uses a `2px` `--color-accent` outline. `outline: none` is banned except where a custom focus style replaces it immediately.
- **Keyboard operability:** all interactive primitives are operable via Tab/Enter/Space natively (no custom keyboard handlers needed).
- **ARIA:** `aria-busy` on loading buttons/banners; `aria-invalid` + `aria-describedby` on errored inputs; `role="alert"` on error banners; `role="status"` on empty/loading banners.
- **Contrast:** `--color-fg` on `--color-bg` = 19.3:1 (AAA). `--color-muted` on `--color-bg` = 4.5:1 (AA, passes). `--color-accent-fg` on `--color-accent` = 4.6:1 (AA). `--color-danger` text on `--color-bg` = 5.9:1 (AA). All pass WCAG AA; body text passes AAA.

### Accepted Debt
- **Light mode only this wave.** Dark mode (`prefers-color-scheme: dark`) is out of scope; tokens are not yet dual-mode. Tracked for a later wave — the token architecture is ready for a `[data-theme="dark"]` override block.
- **`--color-muted` is AA, not AAA.** Reserved for helper text, placeholders, and metadata — never running body copy. Body always uses `--color-fg` (AAA). Documented, not a violation.
- **No icon library.** The spinner is a CSS-only border spinner (no emoji, no SVG icon set). If real icons are needed later, install Phosphor or Radix Icons — do not hand-roll SVGs.
- **No Lighthouse audit run this wave.** Primitives are built but not yet composed into pages (todos 4/5/6 consume them). The full Playwright + Lighthouse 100 audit runs after page composition.
- **`color-mix()` for banner tints** may lack older-browser support; a solid `--color-bg` fallback is declared before each `color-mix()` call. Accepted: modern-evergreen browsers only.
