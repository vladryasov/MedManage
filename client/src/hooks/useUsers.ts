import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getAllUsersExcept, updateUserRole, updateUserPhone, updateUserInfo, createUser, deleteUser } from '../api/users';
import type { UserDTO, UserRole, CreateUserRequest } from '../types';

export function useUsers() {
  return useQuery({
    queryKey: ['users'],
    queryFn: getAllUsersExcept,
  });
}

export function useUpdateUserRole() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ user, newRole }: { user: UserDTO; newRole: UserRole }) =>
      updateUserRole(user, newRole),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['users'] }),
  });
}

export function useUpdateUserPhone() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (user: UserDTO) => updateUserPhone(user),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['currentUser'] });
    },
  });
}

export function useUpdateUserInfo() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (user: UserDTO) => updateUserInfo(user),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['currentUser'] }),
  });
}

export function useDeleteUser() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (userId: string) => deleteUser(userId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['users'] }),
  });
}

export function useCreateUser() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: CreateUserRequest) => createUser(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
    },
  });
}
