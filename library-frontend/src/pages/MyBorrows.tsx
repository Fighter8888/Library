import { useEffect, useState } from "react";
import { borrowsApi } from "../api/client";
import { useAuth } from "../contexts/AuthContext";
import type { Borrow } from "../types";
import "./MyBorrows.css";

export default function MyBorrows() {
  const { auth } = useAuth();
  const [borrows, setBorrows] = useState<Borrow[]>([]);
  const [loading, setLoading] = useState(true);
  const [returning, setReturning] = useState<string | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!auth.isAuthenticated) return;
    borrowsApi
      .myBorrows()
      .then(setBorrows)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [auth.isAuthenticated]);

  const handleReturn = async (borrowId: string) => {
    setError("");
    setReturning(borrowId);
    try {
      await borrowsApi.return({ borrowId });
      setBorrows((prev) => prev.filter((b) => b.id !== borrowId));
    } catch (err) {
      setError(err instanceof Error ? err.message : "خطا در برگرداندن");
    } finally {
      setReturning(null);
    }
  };

  const formatDate = (s: string) => new Date(s).toLocaleDateString("fa-IR");

  if (!auth.isAuthenticated) {
    return <p>برای مشاهده امانت‌ها وارد شوید.</p>;
  }

  if (loading) return <p>در حال بارگذاری...</p>;

  return (
    <div className="my-borrows-page">
      <h1>امانت‌های من</h1>
      {error && <div className="message error">{error}</div>}
      <div className="borrows-list">
        {borrows.map((b) => (
          <div key={b.id} className={`borrow-card ${b.isOverdue ? "overdue" : ""}`}>
            <h3>{b.bookTitle}</h3>
            <p>کد نسخه: {b.inventoryCode}</p>
            <p>امانت: {formatDate(b.borrowedAtUtc)}</p>
            <p>موعد بازگشت: {formatDate(b.dueAtUtc)}</p>
            {b.isOverdue && <span className="badge">تأخیر</span>}
            {!b.returnedAtUtc && (
              <button
                type="button"
                className="btn btn-primary"
                disabled={returning === b.id}
                onClick={() => handleReturn(b.id)}
              >
                {returning === b.id ? "در حال ثبت..." : "برگرداندن کتاب"}
              </button>
            )}
          </div>
        ))}
      </div>
      {borrows.length === 0 && <p>امانتی ندارید.</p>}
    </div>
  );
}
