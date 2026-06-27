import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  getIncomingRequests,
  getOutgoingRequests,
  createPurchaseRequest,
  acceptPurchaseRequest,
  rejectPurchaseRequest,
  deletePurchaseRequest,
} from '../api/purchaseRequests';
import type { CreatePurchaseRequestRequest } from '../types';

export function useIncomingRequests() {
  return useQuery({
    queryKey: ['purchaseRequests', 'incoming'],
    queryFn: getIncomingRequests,
  });
}

export function useOutgoingRequests() {
  return useQuery({
    queryKey: ['purchaseRequests', 'outgoing'],
    queryFn: getOutgoingRequests,
  });
}

export function useCreatePurchaseRequest() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: CreatePurchaseRequestRequest) => createPurchaseRequest(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['purchaseRequests'] });
    },
  });
}

export function useAcceptPurchaseRequest() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => acceptPurchaseRequest(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['purchaseRequests'] });
      queryClient.invalidateQueries({ queryKey: ['announcements'] });
    },
  });
}

export function useRejectPurchaseRequest() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => rejectPurchaseRequest(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['purchaseRequests'] });
    },
  });
}

export function useDeletePurchaseRequest() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deletePurchaseRequest(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['purchaseRequests'] });
    },
  });
}
