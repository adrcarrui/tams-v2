export type DeviceDto = {
  id: number;
  name: string;
  assetVariantId: number;
  assetVariantCode: string;
  assetVariantName: string;
  assetTypeId: number;
  assetTypeCode: string;
  assetTypeName: string;
  managedByDepartmentId: number;
  managedByDepartmentCode: string;
  uid: string | null;
  barcode: string | null;
  serialNumber: string | null;
  status: string;
  isActive: boolean;
  notes: string | null;
  createdAtUtc: string;
  updatedAtUtc: string | null;
};

export type PagedResultDto<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
};

export type CreateDeviceRequest = {
  name: string;
  assetVariantId: number;
  uid: string | null;
  barcode: string | null;
  serialNumber: string | null;
  notes: string | null;
};