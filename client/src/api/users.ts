import apiClient from './axios';
import type { UserDTO, UserRole, CreateUserRequest } from '../types';

export async function getCurrentUser(): Promise<UserDTO> {
  const { data } = await apiClient.get('/User/CurrentUser');
  return data;
}

export async function getAllUsersExcept(): Promise<UserDTO[]> {
  const { data } = await apiClient.get('/User/users/all');
  return data;
}

export async function updateUserInfo(user: UserDTO): Promise<void> {
  await apiClient.put('/User/update', user);
}

export async function getUserName(): Promise<string> {
  const { data } = await apiClient.get('/User/name');
  return data.userName;
}

export async function updateUserRole(
  user: UserDTO,
  newRole: UserRole,
): Promise<void> {
  await apiClient.patch(`/User/users/${user.userId}/Role`, user, {
    params: { newRole },
  });
}

export async function updateUserPhone(
  user: UserDTO,
): Promise<void> {
  await apiClient.patch(`/User/users/${user.userId}/updateNumber`, user);
}

export async function deleteUser(userId: string): Promise<void> {
  await apiClient.delete(`/User/${userId}`);
}

export async function createUser(request: CreateUserRequest): Promise<UserDTO> {
  const { data } = await apiClient.post('/User/create', request);
  return data;
}
