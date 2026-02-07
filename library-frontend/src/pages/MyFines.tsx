import { useEffect, useState } from "react";
import { finesApi } from "../api/client";
import { useAuth } from "../contexts/AuthContext";
import type { Fine } from "../types";
import "./MyFines.css";

export default function MyFines() {
  const { auth } = useAuth();
  const [fines, setFines] = useState<Fine[]>([]);
  const [loading, setLoading] = useState(true);
  const [paying, setPaying] = useState<string | null>(null);

  useEffect(() => {
    if (!auth.isAuthenticated) return;
    finesApi
      .myFines()
      .then(setFines)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [auth.isAuthenticated]);

  const handlePay = async (fineId: string) => {
    setPaying(fineId);
    try {
      await finesApi.pay(fineId);
      setFines((prev) => prev.filter((f) => f.id !== fineId));
    } catch (err) {
      console.error(err);
    } finally {
      setPaying(null);
    }
  };

  const formatDate = (s: string) => new Date(s).toLocaleDateString("fa-IR");
  const unpaid = fines.filter((f) => !f.isPaid);
  const totalUnpaid = unpaid.reduce((sum, f) => sum + f.amount, 0);

  if (!auth.isAuthenticated) {
    return <p>برای مشاهده جریمه‌ها وارد شوید.</p>;
  }

  if (loading) return <p>در حال بارگذاری...</p>;

  return (
    <div className="my-fines-page">
      <h1>جریمه‌های من</h1>
      {unpaid.length > 0 && (
        <p className="total">مجموع پرداخت نشده: {totalUnpaid.toLocaleString("fa-IR")} ریال</p>
      )}
      <div className="fines-list">
        {fines.map((f) => (
          <div key={f.id} className={`fine-card ${f.isPaid ? "paid" : ""}`}>
            <h3>{f.bookTitle}</h3>
            <p>مبلغ: {f.amount.toLocaleString("fa-IR")} ریال</p>
            <p>سررسید: {formatDate(f.dueDate)}</p>
            {f.isPaid ? (
              <span className="badge success">پرداخت شده</span>
            ) : (
              <button
                type="button"
                className="btn btn-primary"
                disabled={paying === f.id}
                onClick={() => handlePay(f.id)}
              >
                {paying === f.id ? "در حال پرداخت..." : "پرداخت"}
              </button>
            )}
          </div>
        ))}
      </div>
      {fines.length === 0 && <p>جریمه‌ای ندارید.</p>}
    </div>
  );
}
