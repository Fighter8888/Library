import { Link, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import "./Layout.css";

export default function Layout() {
  const { auth, logout, isAdmin } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="layout">
      <header className="header">
        <Link to="/" className="logo">
          کتابخانه
        </Link>
        <nav className="nav">
          <Link to="/">کتاب‌ها</Link>
          <Link to="/categories">دسته‌بندی‌ها</Link>
          {auth.isAuthenticated && (
            <>
              <Link to="/my-borrows">امانت‌های من</Link>
              <Link to="/my-reservations">رزروهای من</Link>
              <Link to="/my-fines">جریمه‌های من</Link>
              {isAdmin() && <Link to="/admin">مدیریت</Link>}
            </>
          )}
        </nav>
        <div className="user">
          {auth.isAuthenticated ? (
            <>
              <span className="username">{auth.username}</span>
              <button type="button" onClick={handleLogout} className="btn btn-outline">
                خروج
              </button>
            </>
          ) : (
            <>
              <Link to="/login" className="btn btn-outline">
                ورود
              </Link>
              <Link to="/signup" className="btn btn-primary">
                ثبت‌نام
              </Link>
            </>
          )}
        </div>
      </header>
      <main className="main">
        <Outlet />
      </main>
    </div>
  );
}
