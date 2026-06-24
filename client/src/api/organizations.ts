import apiClient from './axios';
import type { OrganizationDTO } from '../types';

export async function getAllOrganizations(): Promise<OrganizationDTO[]> {
  const { data } = await apiClient.get('/Organization');
  return data;
}

export async function getOrganizationById(id: string): Promise<OrganizationDTO> {
  const { data } = await apiClient.get(`/Organization/${id}`);
  return data;
}

export async function createOrganization(request: {
  name: string;
  address: string;
  phoneNumber: string;
  email: string;
}): Promise<OrganizationDTO> {
  const { data } = await apiClient.post('/Organization', request);
  return data;
}

export async function updateOrganization(dto: OrganizationDTO): Promise<void> {
  await apiClient.put('/Organization', dto);
}

export async function deleteOrganization(id: string): Promise<void> {
  await apiClient.delete(`/Organization/${id}`);
}
