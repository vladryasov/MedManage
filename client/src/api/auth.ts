import apiClient from './axios';
import type { UserDTO } from '../types';

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: UserDTO;
}

export async function login(userName: string, password: string): Promise<AuthResponse> {
  const response = await apiClient.post('/Auth/login', { userName, password });
  return response.data;
}

export async function refresh(refreshToken: string): Promise<AuthResponse> {
  const response = await apiClient.post('/Auth/refresh', { refreshToken });
  return response.data;
}
