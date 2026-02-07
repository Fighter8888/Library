import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { categoriesApi } from "../api/client";
import type { Category } from "../types";
import "./Categories.css";

export default function Categories() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    categoriesApi
      .getAll()
      .then(setCategories)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p>در حال بارگذاری...</p>;

  return (
    <div className="categories-page">
      <h1>دسته‌بندی‌ها</h1>
      <div className="category-grid">
        {categories.map((c) => (
          <Link to={`/?categoryId=${c.id}`} key={c.id} className="category-card">
            <span className="name">{c.name}</span>
            <span className="count">{c.bookCount} کتاب</span>
          </Link>
        ))}
      </div>
      {categories.length === 0 && <p>دسته‌ای یافت نشد.</p>}
    </div>
  );
}
