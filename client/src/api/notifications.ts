import apiClient from './axios';
import type { InAppNotificationDTO } from '../types';

export async function getNotifications(): Promise<InAppNotificationDTO[]> {
  const { data } = await apiClient.get('/Notification');
  return data;
}

export async function getUnreadCount(): Promise<number> {
  const { data } = await apiClient.get('/Notification/unread-count');
  return data.count;
}

export async function markAsRead(id: string): Promise<void> {
  await apiClient.patch(`/Notification/${id}/read`);
}

export async function markAllAsRead(): Promise<void> {
  await apiClient.patch('/Notification/read-all');
}
