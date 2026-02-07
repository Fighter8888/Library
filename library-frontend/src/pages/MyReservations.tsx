import { useEffect, useState } from "react";
import { reservationsApi } from "../api/client";
import { useAuth } from "../contexts/AuthContext";
import type { Reservation } from "../types";
import "./MyReservations.css";

export default function MyReservations() {
  const { auth } = useAuth();
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [loading, setLoading] = useState(true);
  const [action, setAction] = useState<string | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!auth.isAuthenticated) return;
    reservationsApi
      .myReservations()
      .then(setReservations)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [auth.isAuthenticated]);

  const handleCancel = async (id: string) => {
    setError("");
    setAction(id);
    try {
      await reservationsApi.cancel(id);
      setReservations((prev) => prev.filter((r) => r.id !== id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "خطا");
    } finally {
      setAction(null);
    }
  };

  const handleConvertToBorrow = async (id: string) => {
    setError("");
    setAction(id);
    try {
      await reservationsApi.convertToBorrow(id, 14);
      setReservations((prev) => prev.filter((r) => r.id !== id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "خطا");
    } finally {
      setAction(null);
    }
  };

  const formatDate = (s: string) => new Date(s).toLocaleDateString("fa-IR");

  if (!auth.isAuthenticated) {
    return <p>برای مشاهده رزروها وارد شوید.</p>;
  }

  if (loading) return <p>در حال بارگذاری...</p>;

  return (
    <div className="my-reservations-page">
      <h1>رزروهای من</h1>
      {error && <div className="message error">{error}</div>}
      <div className="reservations-list">
        {reservations.map((r) => (
          <div
            key={r.id}
            className={`reservation-card ${!r.isActive || r.isExpired ? "inactive" : ""}`}
          >
            <h3>{r.bookTitle}</h3>
            <p>کد نسخه: {r.inventoryCode}</p>
            <p>تاریخ رزرو: {formatDate(r.createdAtUtc)}</p>
            {r.expiresAtUtc && <p>انقضا: {formatDate(r.expiresAtUtc)}</p>}
            {r.isActive && !r.isExpired && (
              <div className="actions">
                <button
                  type="button"
                  className="btn btn-primary"
                  disabled={!!action}
                  onClick={() => handleConvertToBorrow(r.id)}
                >
                  {action === r.id ? "..." : "تحویل به امانت"}
                </button>
                <button
                  type="button"
                  className="btn btn-outline"
                  disabled={!!action}
                  onClick={() => handleCancel(r.id)}
                >
                  انصراف از رزرو
                </button>
              </div>
            )}
            {(r.isExpired || !r.isActive) && (
              <span className="badge">{r.isExpired ? "منقضی شده" : "لغو شده"}</span>
            )}
          </div>
        ))}
      </div>
      {reservations.length === 0 && <p>رزروی ندارید.</p>}
    </div>
  );
}
