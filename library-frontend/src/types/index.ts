// Auth
export interface AuthResponse {
  token: string;
  username: string;
  roles: string[];
  accountId: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface SignupRequest {
  username: string;
  password: string;
  firstName: string;
  lastName: string;
  email?: string;
}

// Books
export interface Book {
  id: string;
  title: string;
  isbn?: string;
  publicationYear?: number;
  publisherName: string;
  authors: string[];
  categories: string[];
  availableCopies: number;
  totalCopies: number;
}

export interface AvailableCopy {
  id: string;
  inventoryCode: string;
}

export interface Category {
  id: string;
  name: string;
  bookCount: number;
}

export interface AuthorDto {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  biography?: string;
}

export interface PublisherDto {
  id: string;
  name: string;
}

export interface CreateBookRequest {
  title: string;
  isbn?: string;
  publicationYear?: number;
  publisherId: string;
  authorIds: string[];
  categoryIds: string[];
}

// Borrows
export interface Borrow {
  id: string;
  bookTitle: string;
  inventoryCode: string;
  borrowedAtUtc: string;
  dueAtUtc: string;
  returnedAtUtc?: string;
  isOverdue: boolean;
}

export interface BorrowBookRequest {
  availableBookId: string;
  daysToBorrow?: number;
}

export interface ReturnBookRequest {
  borrowId: string;
}

// Fines
export interface Fine {
  id: string;
  borrowId: string;
  bookTitle: string;
  amount: number;
  dueDate: string;
  paidAtUtc?: string;
  isPaid: boolean;
  notes?: string;
}

// Reservations
export interface Reservation {
  id: string;
  bookTitle: string;
  inventoryCode: string;
  createdAtUtc: string;
  expiresAtUtc?: string;
  cancelledAtUtc?: string;
  isActive: boolean;
  isExpired: boolean;
}

export interface CreateReservationRequest {
  availableBookId: string;
  expirationDays?: number;
}
