export const UserRole = {
  All: 0,
  CommonUser: 1,
  Admin: 2,
  SpecialUser: 3,
} as const;
export type UserRole = (typeof UserRole)[keyof typeof UserRole];

export const InventoryStatus = {
  All: 0,
  InStock: 1,
  OutOfStock: 2,
  Replenishing: 3,
  Expired: 4,
} as const;
export type InventoryStatus = (typeof InventoryStatus)[keyof typeof InventoryStatus];

export const ProductType = {
  All: 0,
  Tablet: 1,
  MedicineEquipment: 2,
  Syringe: 3,
  Ventilator: 4,
  SurgicalInstrument: 5,
  Consumable: 6,
} as const;
export type ProductType = (typeof ProductType)[keyof typeof ProductType];

export const TypeOfSort = {
  All: 0,
  ByDate: 1,
  ByCategory: 2,
} as const;
export type TypeOfSort = (typeof TypeOfSort)[keyof typeof TypeOfSort];

export interface AnnouncementDTO {
  announcementId: string;
  title: string;
  content: string;
  createdAt: string;
  expirationDate?: string | null;
  createdByUserId: string;
  statusInventory: number;
  typeProduct: number;
  updatedAt?: string | null;
  userName: string;
  views: number;
}

export interface UserDTO {
  userId: string;
  userName: string;
  fullName: string;
  email: string;
  role: number;
  createdAt: string;
  phoneNumber: string;
}

export interface OrganizationDTO {
  organizationId: string;
  name: string;
  address: string;
  phoneNumber: string;
  email: string;
  createdAt: string;
}

export interface CreateUserRequest {
  email: string;
  fullName: string;
  phoneNumber?: string;
  organizationId?: string;
}

export interface PaginatedQueryParams {
  pageNumber: number;
  pageSize: number;
  sortBy?: number;
  searchFilter?: string;
  productType?: number;
  statusInventory?: number;
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
}

export const inventoryStatusLabels: Record<number, string> = {
  [InventoryStatus.All]: 'Все',
  [InventoryStatus.InStock]: 'В наличии',
  [InventoryStatus.OutOfStock]: 'Нет в наличии',
  [InventoryStatus.Replenishing]: 'Пополняется',
  [InventoryStatus.Expired]: 'Просрочен',
};

export const productTypeLabels: Record<number, string> = {
  [ProductType.All]: 'Все',
  [ProductType.Tablet]: 'Таблетки',
  [ProductType.MedicineEquipment]: 'Мед. оборудование',
  [ProductType.Syringe]: 'Шприцы',
  [ProductType.Ventilator]: 'ИВЛ',
  [ProductType.SurgicalInstrument]: 'Хир. инструменты',
  [ProductType.Consumable]: 'Расходные материалы',
};

export const userRoleLabels: Record<number, string> = {
  [UserRole.All]: 'Все',
  [UserRole.CommonUser]: 'Обычный',
  [UserRole.Admin]: 'Админ',
  [UserRole.SpecialUser]: 'Спец. пользователь',
};

export const typeOfSortLabels: Record<number, string> = {
  [TypeOfSort.All]: 'Без сортировки',
  [TypeOfSort.ByDate]: 'По дате',
  [TypeOfSort.ByCategory]: 'По категории',
};

export const PurchaseRequestStatus = {
  Pending: 0,
  Accepted: 1,
  Rejected: 2,
} as const;
export type PurchaseRequestStatus = (typeof PurchaseRequestStatus)[keyof typeof PurchaseRequestStatus];

export const InAppNotificationType = {
  PurchaseRequest: 0,
  RequestAccepted: 1,
  RequestRejected: 2,
} as const;
export type InAppNotificationType = (typeof InAppNotificationType)[keyof typeof InAppNotificationType];

export interface PurchaseRequestDTO {
  purchaseRequestId: string;
  announcementId?: string | null;
  buyerUserId: string;
  sellerUserId: string;
  status: number;
  message?: string | null;
  createdAt: string;
  updatedAt: string;
  announcementTitle?: string | null;
  buyerUserName: string;
  sellerUserName: string;
}

export interface InAppNotificationDTO {
  id: string;
  recipientUserId: string;
  senderUserId?: string | null;
  title: string;
  message: string;
  type: number;
  relatedEntityId?: string | null;
  isRead: boolean;
  createdAt: string;
  senderUserName?: string | null;
}

export interface CreatePurchaseRequestRequest {
  announcementId: string;
  message?: string;
}

export const purchaseRequestStatusLabels: Record<number, string> = {
  [PurchaseRequestStatus.Pending]: 'Ожидает',
  [PurchaseRequestStatus.Accepted]: 'Принят',
  [PurchaseRequestStatus.Rejected]: 'Отклонён',
};

export const inAppNotificationTypeLabels: Record<number, string> = {
  [InAppNotificationType.PurchaseRequest]: 'Новый запрос',
  [InAppNotificationType.RequestAccepted]: 'Запрос принят',
  [InAppNotificationType.RequestRejected]: 'Запрос отклонён',
};
