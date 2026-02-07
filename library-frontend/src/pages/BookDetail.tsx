import { useEffect, useState } from "react";
import { Link, useParams, useNavigate } from "react-router-dom";
import { booksApi, borrowsApi, reservationsApi } from "../api/client";
import { useAuth } from "../contexts/AuthContext";
import type { Book, AvailableCopy } from "../types";
import "./BookDetail.css";

export default function BookDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { auth } = useAuth();
  const [book, setBook] = useState<Book | null>(null);
  const [copies, setCopies] = useState<AvailableCopy[]>([]);
  const [loading, setLoading] = useState(true);
  const [action, setAction] = useState<"borrow" | "reserve" | null>(null);
  const [days, setDays] = useState(14);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  useEffect(() => {
    if (!id) return;
    setLoading(true);
    Promise.all([booksApi.getById(id), booksApi.getAvailableCopies(id)])
      .then(([b, c]) => {
        setBook(b);
        setCopies(c);
      })
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [id]);

  const handleBorrow = async (copyId: string) => {
    if (!auth.isAuthenticated) {
      navigate("/login");
      return;
    }
    setError("");
    setSuccess("");
    try {
      await borrowsApi.borrow({ availableBookId: copyId, daysToBorrow: days });
      setSuccess("کتاب با موفقیت امانت گرفته شد.");
      setAction(null);
      const [b, c] = await Promise.all([
        booksApi.getById(id!),
        booksApi.getAvailableCopies(id!),
      ]);
      setBook(b);
      setCopies(c);
    } catch (err) {
      setError(err instanceof Error ? err.message : "خطا");
    }
  };

  const handleReserve = async (copyId: string) => {
    if (!auth.isAuthenticated) {
      navigate("/login");
      return;
    }
    setError("");
    setSuccess("");
    try {
      await reservationsApi.create({ availableBookId: copyId, expirationDays: 7 });
      setSuccess("رزرو ثبت شد.");
      setAction(null);
      const [b, c] = await Promise.all([
        booksApi.getById(id!),
        booksApi.getAvailableCopies(id!),
      ]);
      setBook(b);
      setCopies(c);
    } catch (err) {
      setError(err instanceof Error ? err.message : "خطا");
    }
  };

  if (loading || !book) {
    return <div className="book-detail-page">{loading ? "در حال بارگذاری..." : "کتاب یافت نشد."}</div>;
  }

  return (
    <div className="book-detail-page">
      <div className="book-detail-card">
        <h1>{book.title}</h1>
        <p className="meta">
          نویسنده: {book.authors.join("، ")}
          {book.publicationYear && ` • سال: ${book.publicationYear}`}
        </p>
        {book.isbn && <p>شابک: {book.isbn}</p>}
        <p>ناشر: {book.publisherName}</p>
        <p className="categories">دسته‌ها: {book.categories.join("، ")}</p>
        <p className="copies">
          موجود: {book.availableCopies} از {book.totalCopies}
        </p>

        {error && <div className="message error">{error}</div>}
        {success && <div className="message success">{success}</div>}

        {auth.isAuthenticated && copies.length > 0 && (
          <div className="actions">
            {action === "borrow" ? (
              <div className="action-form">
                <label>
                  تعداد روز امانت:
                  <input
                    type="number"
                    min={1}
                    max={30}
                    value={days}
                    onChange={(e) => setDays(Number(e.target.value))}
                  />
                </label>
                <div className="copy-buttons">
                  {copies.map((c) => (
                    <button
                      key={c.id}
                      type="button"
                      className="btn btn-primary"
                      onClick={() => handleBorrow(c.id)}
                    >
                      امانت (نسخه {c.inventoryCode})
                    </button>
                  ))}
                </div>
                <button type="button" className="btn btn-outline" onClick={() => setAction(null)}>
                  انصراف
                </button>
              </div>
            ) : action === "reserve" ? (
              <div className="action-form">
                <div className="copy-buttons">
                  {copies.map((c) => (
                    <button
                      key={c.id}
                      type="button"
                      className="btn btn-primary"
                      onClick={() => handleReserve(c.id)}
                    >
                      رزرو (نسخه {c.inventoryCode})
                    </button>
                  ))}
                </div>
                <button type="button" className="btn btn-outline" onClick={() => setAction(null)}>
                  انصراف
                </button>
              </div>
            ) : (
              <>
                <button type="button" className="btn btn-primary" onClick={() => setAction("borrow")}>
                  امانت گرفتن
                </button>
                <button type="button" className="btn btn-outline" onClick={() => setAction("reserve")}>
                  رزرو
                </button>
              </>
            )}
          </div>
        )}

        {!auth.isAuthenticated && copies.length > 0 && (
          <p className="hint">
            برای امانت یا رزرو <Link to="/login">وارد</Link> شوید.
          </p>
        )}
      </div>
    </div>
  );
}
