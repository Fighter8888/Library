const API_BASE = import.meta.env.VITE_API_URL ?? "http://localhost:5113";

function getToken(): string | null {
  return localStorage.getItem("token");
}

export async function api<T>(
  path: string,
  options: RequestInit = {}
): Promise<T> {
  const token = getToken();
  const headers: HeadersInit = {
    "Content-Type": "application/json",
    ...(options.headers as Record<string, string>),
  };
  if (token) {
    (headers as Record<string, string>)["Authorization"] = `Bearer ${token}`;
  }
  const res = await fetch(`${API_BASE}${path}`, { ...options, headers });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `HTTP ${res.status}`);
  }
  if (res.status === 204) return undefined as T;
  return res.json() as Promise<T>;
}

// Auth
export const authApi = {
  login: (body: { username: string; password: string }) =>
    api<import("../types").AuthResponse>("/api/auth/login", {
      method: "POST",
      body: JSON.stringify(body),
    }),
  signup: (body: import("../types").SignupRequest) =>
    api<import("../types").AuthResponse>("/api/auth/signup", {
      method: "POST",
      body: JSON.stringify(body),
    }),
};

// Books
export const booksApi = {
  getAll: () => api<import("../types").Book[]>("/api/books"),
  search: (params: { title?: string; author?: string; isbn?: string; categoryId?: string }) => {
    const q = new URLSearchParams();
    if (params.title) q.set("title", params.title);
    if (params.author) q.set("author", params.author);
    if (params.isbn) q.set("isbn", params.isbn);
    if (params.categoryId) q.set("categoryId", params.categoryId);
    return api<import("../types").Book[]>(`/api/books/search?${q}`);
  },
  getById: (id: string) => api<import("../types").Book>(`/api/books/${id}`),
  getAvailableCopies: (bookId: string) =>
    api<import("../types").AvailableCopy[]>(`/api/books/${bookId}/available-copies`),
  create: (body: import("../types").CreateBookRequest) =>
    api<string>("/api/books", { method: "POST", body: JSON.stringify(body) }),
  getAuthors: () => api<import("../types").AuthorDto[]>("/api/books/authors"),
  getPublishers: () => api<import("../types").PublisherDto[]>("/api/books/publishers"),
  createAuthor: (body: { firstName: string; lastName: string; biography?: string }) =>
    api<string>("/api/books/authors", { method: "POST", body: JSON.stringify(body) }),
  createPublisher: (body: { name: string }) =>
    api<string>("/api/books/publishers", { method: "POST", body: JSON.stringify(body) }),
  addInventory: (body: { bookId: string; inventoryCode: string }) =>
    api<string>("/api/books/inventory", { method: "POST", body: JSON.stringify(body) }),
};

// Categories
export const categoriesApi = {
  getAll: () => api<import("../types").Category[]>("/api/categories"),
  getById: (id: string) => api<import("../types").Category>(`/api/categories/${id}`),
  create: (body: { name: string }) =>
    api<string>("/api/categories", { method: "POST", body: JSON.stringify(body) }),
  update: (id: string, body: { name: string }) =>
    api<void>(`/api/categories/${id}`, { method: "PUT", body: JSON.stringify(body) }),
  delete: (id: string) => api<void>(`/api/categories/${id}`, { method: "DELETE" }),
};

// Borrows
export const borrowsApi = {
  borrow: (body: import("../types").BorrowBookRequest) =>
    api<string>("/api/borrows", { method: "POST", body: JSON.stringify(body) }),
  return: (body: import("../types").ReturnBookRequest) =>
    api<void>("/api/borrows/return", { method: "POST", body: JSON.stringify(body) }),
  myBorrows: () => api<import("../types").Borrow[]>("/api/borrows/my-borrows"),
};

// Fines
export const finesApi = {
  myFines: () => api<import("../types").Fine[]>("/api/fines/my-fines"),
  pay: (fineId: string) =>
    api<void>(`/api/fines/${fineId}/pay`, { method: "POST" }),
};

// Reservations
export const reservationsApi = {
  myReservations: () =>
    api<import("../types").Reservation[]>("/api/reservations/my-reservations"),
  create: (body: import("../types").CreateReservationRequest) =>
    api<string>("/api/reservations", { method: "POST", body: JSON.stringify(body) }),
  cancel: (reservationId: string) =>
    api<void>(`/api/reservations/${reservationId}/cancel`, { method: "POST" }),
  convertToBorrow: (reservationId: string, daysToBorrow?: number) => {
    const q = daysToBorrow != null ? `?daysToBorrow=${daysToBorrow}` : "";
    return api<string>(`/api/reservations/${reservationId}/convert-to-borrow${q}`, {
      method: "POST",
    });
  },
};
