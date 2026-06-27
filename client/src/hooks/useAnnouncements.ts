import { useQuery } from '@tanstack/react-query';
import {
  getAllAnnouncements,
  getAnnouncementById,
  getPaginatedAnnouncements,
  getMyAnnouncements,
  createAnnouncement,
  updateAnnouncementContent,
  deleteAnnouncement,
} from '../api/announcements';
import type { PaginatedQueryParams } from '../types';

export function useAnnouncements() {
  return useQuery({
    queryKey: ['announcements'],
    queryFn: getAllAnnouncements,
    staleTime: 0,
  });
}

export function useAnnouncement(id: string) {
  return useQuery({
    queryKey: ['announcement', id],
    queryFn: () => getAnnouncementById(id),
    enabled: !!id,
    staleTime: 0,
  });
}

export function useMyAnnouncements() {
  return useQuery({
    queryKey: ['announcements', 'my'],
    queryFn: getMyAnnouncements,
  });
}

export function usePaginatedAnnouncements(params: PaginatedQueryParams) {
  return useQuery({
    queryKey: ['announcements', 'paginated', params],
    queryFn: () => getPaginatedAnnouncements(params),
    staleTime: 10_000,
  });
}

export { createAnnouncement, updateAnnouncementContent, deleteAnnouncement };
