# Library Management – Frontend

React SPA for the library system. Talks to the ASP.NET Core API in the repo root (`Library`). Users browse/search books, borrow, reserve, return, and pay fines; admins manage categories, authors, publishers, books, and inventory.

**Stack:** React 19, TypeScript, Vite 7, React Router. Auth via JWT in `AuthContext`; API calls in `src/api/client.ts`.

---

## Run

1. Start the backend (from repo root): `cd Library && dotnet run`
2. Install and run frontend:
   ```bash
   npm install
   npm run dev
   ```
3. Open **http://localhost:5173**

If the API runs on another port, add a `.env` file:
```env
VITE_API_URL=http://localhost:5113
```

---

## Scripts

| Command        | Description        |
|----------------|--------------------|
| `npm run dev`  | Dev server + HMR   |
| `npm run build`| Production build   |
| `npm run preview` | Serve `dist/`   |
| `npm run lint` | ESLint             |

---

## Admin

Default admin (seeded by backend): **username** `admin`, **password** `Admin@123!ChangeMe`. Change in production.

---

## Structure

- `src/api/client.ts` – API client and endpoints
- `src/contexts/AuthContext.tsx` – Auth state and JWT
- `src/pages/` – BookList, BookDetail, Categories, Login, Signup, MyBorrows, MyFines, MyReservations, Admin
- `src/components/Layout.tsx` – Header and nav
