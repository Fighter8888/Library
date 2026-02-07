import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider } from "./contexts/AuthContext";
import Layout from "./components/Layout";
import BookList from "./pages/BookList";
import BookDetail from "./pages/BookDetail";
import Categories from "./pages/Categories";
import Login from "./pages/Login";
import Signup from "./pages/Signup";
import MyBorrows from "./pages/MyBorrows";
import MyFines from "./pages/MyFines";
import MyReservations from "./pages/MyReservations";
import Admin from "./pages/Admin";

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<BookList />} />
            <Route path="books/:id" element={<BookDetail />} />
            <Route path="categories" element={<Categories />} />
            <Route path="login" element={<Login />} />
            <Route path="signup" element={<Signup />} />
            <Route path="my-borrows" element={<MyBorrows />} />
            <Route path="my-fines" element={<MyFines />} />
            <Route path="my-reservations" element={<MyReservations />} />
            <Route path="admin" element={<Admin />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
