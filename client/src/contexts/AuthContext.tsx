import { createContext, useContext, useState, useEffect, useCallback, useRef, type ReactNode } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import { getCurrentUser } from '../api/users';
import type { UserDTO } from '../types';

interface AuthContextType {
  user: UserDTO | null;
  loading: boolean;
  loginUser: (user: UserDTO) => void;
  logout: () => void;
  updateUser: (user: UserDTO) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
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
      setUser(null);
    } finally {
      setLoading(false);
      fetchingRef.current = false;
    }
  }, []);

  useEffect(() => {
    fetchUser();
  }, [fetchUser]);

  useEffect(() => {
    const handler = () => {
      setUser(null);
      queryClient.clear();
      navigate('/login');
    };
    window.addEventListener('auth:unauthorized', handler);
    return () => window.removeEventListener('auth:unauthorized', handler);
  }, [navigate, queryClient]);

  const loginUser = (newUser: UserDTO) => {
    setUser(newUser);
    setLoading(false);
    queryClient.invalidateQueries({ queryKey: ['announcements'] });
    queryClient.invalidateQueries({ queryKey: ['users'] });
    queryClient.invalidateQueries({ queryKey: ['notifications'] });
  };

  const logout = async () => {
    try {
      await fetch('/api/Auth/logout', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: '{}',
      });
    } catch {
      // ignore
    }
    queryClient.clear();
    setUser(null);
    navigate('/login');
  };

  const updateUser = (updatedUser: UserDTO) => {
    setUser(updatedUser);
  };

  return (
    <AuthContext.Provider value={{ user, loading, loginUser, logout, updateUser }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
