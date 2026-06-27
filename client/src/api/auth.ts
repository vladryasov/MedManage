import apiClient from './axios';
import type { UserDTO } from '../types';

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: UserDTO;
}

async function sha256Hex(input: string): Promise<string> {
  const encoder = new TextEncoder();
  const data = encoder.encode(input);
  const hashBuffer = await crypto.subtle.digest('SHA-256', data);
  const hashArray = new Uint8Array(hashBuffer);
  return Array.from(hashArray).map((b) => b.toString(16).padStart(2, '0')).join('');
}

export async function login(userName: string, password: string): Promise<AuthResponse> {
  const passwordHash = await sha256Hex(password);
  const response = await apiClient.post('/Auth/login', { userName, password: passwordHash });
  return response.data;
}


