import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { booksApi } from "../api/client";
import { categoriesApi } from "../api/client";
import type { Book, Category } from "../types";
import "./BookList.css";

export default function BookList() {
  const [searchParams] = useSearchParams();
  const categoryFromUrl = searchParams.get("categoryId") ?? "";
  const [books, setBooks] = useState<Book[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState({ title: "", author: "", categoryId: categoryFromUrl });

  useEffect(() => {
    setSearch((s) => ({ ...s, categoryId: categoryFromUrl }));
  }, [categoryFromUrl]);

  useEffect(() => {
    categoriesApi.getAll().then(setCategories).catch(console.error);
  }, []);

  useEffect(() => {
    setLoading(true);
    const hasSearch = search.title || search.author || search.categoryId;
    const promise = hasSearch
      ? booksApi.search({
          title: search.title || undefined,
          author: search.author || undefined,
          categoryId: search.categoryId || undefined,
        })
      : booksApi.getAll();
    promise
      .then(setBooks)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [search.title, search.author, search.categoryId]);

  return (
    <div className="book-list-page">
      <h1>کتاب‌ها</h1>
      <div className="search-bar">
        <input
          type="text"
          placeholder="عنوان کتاب"
          value={search.title}
          onChange={(e) => setSearch((s) => ({ ...s, title: e.target.value }))}
        />
        <input
          type="text"
          placeholder="نویسنده"
          value={search.author}
          onChange={(e) => setSearch((s) => ({ ...s, author: e.target.value }))}
        />
        <select
          value={search.categoryId}
          onChange={(e) => setSearch((s) => ({ ...s, categoryId: e.target.value }))}
        >
          <option value="">همه دسته‌ها</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>
      </div>
      {loading ? (
        <p>در حال بارگذاری...</p>
      ) : (
        <div className="book-grid">
          {books.map((book) => (
            <Link to={`/books/${book.id}`} key={book.id} className="book-card">
              <h3>{book.title}</h3>
              <p className="meta">
                {book.authors.join("، ")}
                {book.publicationYear && ` • ${book.publicationYear}`}
              </p>
              <p className="categories">{book.categories.join("، ")}</p>
              <p className="copies">
                موجود: {book.availableCopies} از {book.totalCopies}
              </p>
            </Link>
          ))}
        </div>
      )}
      {!loading && books.length === 0 && <p>کتابی یافت نشد.</p>}
    </div>
  );
}
