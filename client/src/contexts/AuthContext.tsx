import { createContext, useContext, useState, useEffect, useCallback, useRef, type ReactNode } from 'react';
import { useNavigate } from 'react-router-dom';
import { getCurrentUser } from '../api/users';
import { isTokenExpired } from '../utils/jwt';
import type { UserDTO } from '../types';

interface AuthContextType {
  token: string | null;
  user: UserDTO | null;
  loading: boolean;
  loginUser: (token: string, user: UserDTO) => void;
  logout: () => void;
  updateUser: (user: UserDTO) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const navigate = useNavigate();
  const [token, setTokenState] = useState<string | null>(() => {
    const stored = localStorage.getItem('jwt_token');
    if (stored && isTokenExpired(stored)) {
      localStorage.removeItem('jwt_token');
      return null;
    }
    return stored;
  });
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
      localStorage.removeItem('jwt_token');
      setTokenState(null);
      setUser(null);
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

  useEffect(() => {
    const handler = () => {
      localStorage.removeItem('jwt_token');
      setTokenState(null);
      setUser(null);
      navigate('/login');
    };
    window.addEventListener('auth:unauthorized', handler);
    return () => window.removeEventListener('auth:unauthorized', handler);
  }, [navigate]);

  const loginUser = (newToken: string, newUser: UserDTO) => {
    localStorage.setItem('jwt_token', newToken);
    setTokenState(newToken);
    setUser(newUser);
    setLoading(false);
  };

  const logout = () => {
    localStorage.removeItem('jwt_token');
    setTokenState(null);
    setUser(null);
    navigate('/login');
  };

  const updateUser = (updatedUser: UserDTO) => {
    setUser(updatedUser);
  };

  return (
    <AuthContext.Provider value={{ token, user, loading, loginUser, logout, updateUser }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
