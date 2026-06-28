import apiClient from './axios';
import type { PurchaseRequestDTO, CreatePurchaseRequestRequest } from '../types';

export async function createPurchaseRequest(request: CreatePurchaseRequestRequest): Promise<PurchaseRequestDTO> {
  const { data } = await apiClient.post('/PurchaseRequest', request);
  return data;
}

export async function getIncomingRequests(): Promise<PurchaseRequestDTO[]> {
  const { data } = await apiClient.get('/PurchaseRequest/incoming');
  return data;
}

export async function getOutgoingRequests(): Promise<PurchaseRequestDTO[]> {
  const { data } = await apiClient.get('/PurchaseRequest/outgoing');
  return data;
}

export async function acceptPurchaseRequest(id: string): Promise<void> {
  await apiClient.patch(`/PurchaseRequest/${id}/accept`);
}

export async function rejectPurchaseRequest(id: string): Promise<void> {
  await apiClient.patch(`/PurchaseRequest/${id}/reject`);
}

export async function deletePurchaseRequest(id: string): Promise<void> {
  await apiClient.delete(`/PurchaseRequest/${id}`);
}
