import { useEffect, useState } from "react";
import { booksApi, categoriesApi } from "../api/client";
import { useAuth } from "../contexts/AuthContext";
import type { Category, AuthorDto, PublisherDto, CreateBookRequest } from "../types";
import "./Admin.css";

export default function Admin() {
  const { auth, isAdmin } = useAuth();
  const [tab, setTab] = useState<"categories" | "book">("categories");
  const [categories, setCategories] = useState<Category[]>([]);
  const [authors, setAuthors] = useState<AuthorDto[]>([]);
  const [publishers, setPublishers] = useState<PublisherDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");
  const [newCategory, setNewCategory] = useState("");
  const [newAuthor, setNewAuthor] = useState({ firstName: "", lastName: "", biography: "" });
  const [newPublisher, setNewPublisher] = useState("");
  const [bookForm, setBookForm] = useState<CreateBookRequest>({
    title: "",
    isbn: "",
    publicationYear: undefined,
    publisherId: "",
    authorIds: [],
    categoryIds: [],
  });
  const [inventoryForm, setInventoryForm] = useState({ bookId: "", inventoryCode: "" });

  useEffect(() => {
    if (!isAdmin()) return;
    categoriesApi.getAll().then(setCategories).catch(console.error);
    booksApi.getAuthors().then(setAuthors).catch(console.error);
    booksApi.getPublishers().then(setPublishers).catch(console.error);
  }, [isAdmin]);

  const addCategory = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newCategory.trim()) return;
    setLoading(true);
    setMessage("");
    try {
      await categoriesApi.create({ name: newCategory.trim() });
      setNewCategory("");
      setCategories(await categoriesApi.getAll());
      setMessage("دسته اضافه شد.");
    } catch (err) {
      setMessage(err instanceof Error ? err.message : "خطا");
    } finally {
      setLoading(false);
    }
  };

  const addAuthor = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newAuthor.firstName.trim() || !newAuthor.lastName.trim()) return;
    setLoading(true);
    setMessage("");
    try {
      await booksApi.createAuthor(newAuthor);
      setNewAuthor({ firstName: "", lastName: "", biography: "" });
      setAuthors(await booksApi.getAuthors());
      setMessage("نویسنده اضافه شد.");
    } catch (err) {
      setMessage(err instanceof Error ? err.message : "خطا");
    } finally {
      setLoading(false);
    }
  };

  const addPublisher = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newPublisher.trim()) return;
    setLoading(true);
    setMessage("");
    try {
      await booksApi.createPublisher({ name: newPublisher.trim() });
      setNewPublisher("");
      setPublishers(await booksApi.getPublishers());
      setMessage("ناشر اضافه شد.");
    } catch (err) {
      setMessage(err instanceof Error ? err.message : "خطا");
    } finally {
      setLoading(false);
    }
  };

  const addBook = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!bookForm.title.trim() || !bookForm.publisherId) return;
    if (bookForm.authorIds.length === 0 || bookForm.categoryIds.length === 0) {
      setMessage("نویسنده و دسته‌بندی انتخاب کنید.");
      return;
    }
    setLoading(true);
    setMessage("");
    try {
      await booksApi.create({
        ...bookForm,
        publicationYear: bookForm.publicationYear || undefined,
        isbn: bookForm.isbn || undefined,
      });
      setBookForm({
        title: "",
        isbn: "",
        publicationYear: undefined,
        publisherId: "",
        authorIds: [],
        categoryIds: [],
      });
      setMessage("کتاب اضافه شد.");
    } catch (err) {
      setMessage(err instanceof Error ? err.message : "خطا");
    } finally {
      setLoading(false);
    }
  };

  const addInventory = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!inventoryForm.bookId.trim() || !inventoryForm.inventoryCode.trim()) return;
    setLoading(true);
    setMessage("");
    try {
      await booksApi.addInventory(inventoryForm);
      setInventoryForm({ bookId: "", inventoryCode: "" });
      setMessage("نسخه به انبار اضافه شد.");
    } catch (err) {
      setMessage(err instanceof Error ? err.message : "خطا");
    } finally {
      setLoading(false);
    }
  };

  const toggleAuthor = (id: string) => {
    setBookForm((f) => ({
      ...f,
      authorIds: f.authorIds.includes(id) ? f.authorIds.filter((x) => x !== id) : [...f.authorIds, id],
    }));
  };

  const toggleCategory = (id: string) => {
    setBookForm((f) => ({
      ...f,
      categoryIds: f.categoryIds.includes(id)
        ? f.categoryIds.filter((x) => x !== id)
        : [...f.categoryIds, id],
    }));
  };

  if (!auth.isAuthenticated || !isAdmin()) {
    return <p>دسترسی غیرمجاز.</p>;
  }

  return (
    <div className="admin-page">
      <h1>مدیریت</h1>
      <div className="tabs">
        <button
          type="button"
          className={tab === "categories" ? "active" : ""}
          onClick={() => setTab("categories")}
        >
          دسته‌ها و کتاب
        </button>
        <button
          type="button"
          className={tab === "book" ? "active" : ""}
          onClick={() => setTab("book")}
        >
          افزودن کتاب / نسخه
        </button>
      </div>
      {message && <div className="admin-message">{message}</div>}

      {tab === "categories" && (
        <div className="admin-section">
          <h2>دسته‌بندی جدید</h2>
          <form onSubmit={addCategory} className="admin-form">
            <input
              value={newCategory}
              onChange={(e) => setNewCategory(e.target.value)}
              placeholder="نام دسته"
            />
            <button type="submit" className="btn btn-primary" disabled={loading}>
              افزودن
            </button>
          </form>
          <h2>نویسنده جدید</h2>
          <form onSubmit={addAuthor} className="admin-form multi">
            <input
              value={newAuthor.firstName}
              onChange={(e) => setNewAuthor((a) => ({ ...a, firstName: e.target.value }))}
              placeholder="نام"
            />
            <input
              value={newAuthor.lastName}
              onChange={(e) => setNewAuthor((a) => ({ ...a, lastName: e.target.value }))}
              placeholder="نام خانوادگی"
            />
            <input
              value={newAuthor.biography}
              onChange={(e) => setNewAuthor((a) => ({ ...a, biography: e.target.value }))}
              placeholder="زندگی‌نامه (اختیاری)"
            />
            <button type="submit" className="btn btn-primary" disabled={loading}>
              افزودن
            </button>
          </form>
          <h2>ناشر جدید</h2>
          <form onSubmit={addPublisher} className="admin-form">
            <input
              value={newPublisher}
              onChange={(e) => setNewPublisher(e.target.value)}
              placeholder="نام ناشر"
            />
            <button type="submit" className="btn btn-primary" disabled={loading}>
              افزودن
            </button>
          </form>
        </div>
      )}

      {tab === "book" && (
        <div className="admin-section">
          <h2>کتاب جدید</h2>
          <form onSubmit={addBook} className="admin-form vertical">
            <label>
              عنوان *
              <input
                value={bookForm.title}
                onChange={(e) => setBookForm((f) => ({ ...f, title: e.target.value }))}
                required
              />
            </label>
            <label>
              شابک
              <input
                value={bookForm.isbn}
                onChange={(e) => setBookForm((f) => ({ ...f, isbn: e.target.value }))}
              />
            </label>
            <label>
              سال انتشار
              <input
                type="number"
                value={bookForm.publicationYear ?? ""}
                onChange={(e) =>
                  setBookForm((f) => ({
                    ...f,
                    publicationYear: e.target.value ? Number(e.target.value) : undefined,
                  }))
                }
              />
            </label>
            <label>
              ناشر *
              <select
                value={bookForm.publisherId}
                onChange={(e) => setBookForm((f) => ({ ...f, publisherId: e.target.value }))}
                required
              >
                <option value="">انتخاب کنید</option>
                {publishers.map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.name}
                  </option>
                ))}
              </select>
            </label>
            <div className="checkbox-group">
              <span>نویسندگان *</span>
              {authors.map((a) => (
                <label key={a.id} className="checkbox">
                  <input
                    type="checkbox"
                    checked={bookForm.authorIds.includes(a.id)}
                    onChange={() => toggleAuthor(a.id)}
                  />
                  {a.fullName}
                </label>
              ))}
            </div>
            <div className="checkbox-group">
              <span>دسته‌ها *</span>
              {categories.map((c) => (
                <label key={c.id} className="checkbox">
                  <input
                    type="checkbox"
                    checked={bookForm.categoryIds.includes(c.id)}
                    onChange={() => toggleCategory(c.id)}
                  />
                  {c.name}
                </label>
              ))}
            </div>
            <button type="submit" className="btn btn-primary" disabled={loading}>
              افزودن کتاب
            </button>
          </form>
          <h2>افزودن نسخه به انبار</h2>
          <form onSubmit={addInventory} className="admin-form multi">
            <input
              value={inventoryForm.bookId}
              onChange={(e) => setInventoryForm((f) => ({ ...f, bookId: e.target.value }))}
              placeholder="شناسه کتاب (GUID)"
            />
            <input
              value={inventoryForm.inventoryCode}
              onChange={(e) => setInventoryForm((f) => ({ ...f, inventoryCode: e.target.value }))}
              placeholder="کد نسخه"
            />
            <button type="submit" className="btn btn-primary" disabled={loading}>
              افزودن نسخه
            </button>
          </form>
        </div>
      )}
    </div>
  );
}
