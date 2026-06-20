import apiClient from './axios';
import type { UserDTO } from '../types';

export async function login(token: string): Promise<UserDTO> {
  const response = await apiClient.post('/Auth/login', { token });
  return response.data;
}
