import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getAllOrganizations, createOrganization, updateOrganization, deleteOrganization } from '../api/organizations';
import type { OrganizationDTO } from '../types';

export function useOrganizations() {
  return useQuery({
    queryKey: ['organizations'],
    queryFn: getAllOrganizations,
  });
}

export function useCreateOrganization() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: { name: string; address: string; phoneNumber: string; email: string }) =>
      createOrganization(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['organizations'] }),
  });
}

export function useUpdateOrganization() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (dto: OrganizationDTO) => updateOrganization(dto),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['organizations'] }),
  });
}

export function useDeleteOrganization() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteOrganization(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['organizations'] }),
  });
}
