import { createContext, useCallback, useContext, useState } from "react";
import type { AuthResponse } from "../types";

interface AuthState {
  token: string | null;
  username: string | null;
  roles: string[];
  accountId: string | null;
  isAuthenticated: boolean;
}

const defaultState: AuthState = {
  token: localStorage.getItem("token"),
  username: localStorage.getItem("username"),
  roles: JSON.parse(localStorage.getItem("roles") ?? "[]"),
  accountId: localStorage.getItem("accountId"),
  isAuthenticated: !!localStorage.getItem("token"),
};

const AuthContext = createContext<{
  auth: AuthState;
  login: (r: AuthResponse) => void;
  logout: () => void;
  isAdmin: () => boolean;
}>({
  auth: defaultState,
  login: () => {},
  logout: () => {},
  isAdmin: () => false,
});

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [auth, setAuth] = useState<AuthState>(defaultState);

  const login = useCallback((r: AuthResponse) => {
    localStorage.setItem("token", r.token);
    localStorage.setItem("username", r.username);
    localStorage.setItem("roles", JSON.stringify(r.roles));
    localStorage.setItem("accountId", r.accountId);
    setAuth({
      token: r.token,
      username: r.username,
      roles: r.roles,
      accountId: r.accountId,
      isAuthenticated: true,
    });
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem("token");
    localStorage.removeItem("username");
    localStorage.removeItem("roles");
    localStorage.removeItem("accountId");
    setAuth({
      token: null,
      username: null,
      roles: [],
      accountId: null,
      isAuthenticated: false,
    });
  }, []);

  const isAdmin = useCallback(() => auth.roles.includes("Admin"), [auth.roles]);

  return (
    <AuthContext.Provider value={{ auth, login, logout, isAdmin }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
