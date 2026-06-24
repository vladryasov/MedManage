import { createContext, useContext, useState, useEffect, useCallback, useRef, type ReactNode } from 'react';
import { useNavigate } from 'react-router-dom';
import { getCurrentUser } from '../api/users';
import { STORAGE_KEYS } from '../api/axios';
import type { UserDTO } from '../types';

interface AuthContextType {
  token: string | null;
  user: UserDTO | null;
  loading: boolean;
  loginUser: (token: string, refreshToken: string, user: UserDTO) => void;
  setTokens: (token: string, refreshToken: string) => void;
  logout: () => void;
  updateUser: (user: UserDTO) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const navigate = useNavigate();
  const [token, setTokenState] = useState<string | null>(() =>
    localStorage.getItem(STORAGE_KEYS.TOKEN),
  );
  const [user, setUser] = useState<UserDTO | null>(null);
  const [loading, setLoading] = useState(true);
  const fetchingRef = useRef(false);

  const fetchUser = useCallback(async () => {
    if (fetchingRef.current) return;
    fetchingRef.current = true;
    try {
      const currentUser = await getCurrentUser();
      setUser(currentUser);
    } catch {
      clearTokens();
    } finally {
      setLoading(false);
      fetchingRef.current = false;
    }
  }, []);

  useEffect(() => {
    if (token) {
      fetchUser();
    } else {
      setLoading(false);
    }
  }, [token, fetchUser]);

  const clearTokens = () => {
    localStorage.removeItem(STORAGE_KEYS.TOKEN);
    localStorage.removeItem(STORAGE_KEYS.REFRESH);
    setTokenState(null);
    setUser(null);
  };

  useEffect(() => {
    const handler = () => {
      clearTokens();
      navigate('/login');
    };
    window.addEventListener('auth:unauthorized', handler);
    return () => window.removeEventListener('auth:unauthorized', handler);
  }, [navigate]);

  const loginUser = (newToken: string, newRefresh: string, newUser: UserDTO) => {
    localStorage.setItem(STORAGE_KEYS.TOKEN, newToken);
    localStorage.setItem(STORAGE_KEYS.REFRESH, newRefresh);
    setTokenState(newToken);
    setUser(newUser);
    setLoading(false);
  };

  const setTokens = (newToken: string, newRefresh: string) => {
    localStorage.setItem(STORAGE_KEYS.TOKEN, newToken);
    localStorage.setItem(STORAGE_KEYS.REFRESH, newRefresh);
    setTokenState(newToken);
  };

  const logout = () => {
    clearTokens();
    navigate('/login');
  };

  const updateUser = (updatedUser: UserDTO) => {
    setUser(updatedUser);
  };

  return (
    <AuthContext.Provider value={{ token, user, loading, loginUser, setTokens, logout, updateUser }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
