import apiClient from './axios';
import type { AnnouncementDTO, PaginatedQueryParams, PaginatedResult } from '../types';

export async function getAllAnnouncements(): Promise<AnnouncementDTO[]> {
  const { data } = await apiClient.get('/Announcement/all');
  return data;
}

export async function getAnnouncementById(id: string): Promise<AnnouncementDTO> {
  const { data } = await apiClient.get(`/Announcement/${id}`);
  return data;
}

export async function getPaginatedAnnouncements(
  params: PaginatedQueryParams,
): Promise<PaginatedResult<AnnouncementDTO>> {
  const { data } = await apiClient.get('/Announcement/paginated', { params });
  return data;
}

export async function createAnnouncement(
  announcement: Omit<AnnouncementDTO, 'announcementId' | 'createdAt' | 'updatedAt' | 'userName' | 'views' | 'createdByUserId'>,
): Promise<AnnouncementDTO> {
  const { data } = await apiClient.post('/Announcement/create', announcement);
  return data;
}

export async function updateAnnouncementContent(
  id: string,
  content: string,
): Promise<void> {
  await apiClient.patch(`/Announcement/${id}`, { content });
}

export async function deleteAnnouncement(id: string): Promise<void> {
  await apiClient.delete(`/Announcement/${id}`);
}

export async function getMyAnnouncements(): Promise<AnnouncementDTO[]> {
  const { data } = await apiClient.get('/Announcement/my');
  return data;
}
