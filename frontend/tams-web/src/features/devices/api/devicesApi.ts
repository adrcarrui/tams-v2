import { getJson, postJson, putJson } from "../../../api/httpClient";
import type {
  CreateDeviceRequest,
  DeviceDto,
  PagedResultDto,
  UpdateDeviceRequest,
} from "../types/deviceTypes";

export type GetDevicesParams = {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: string;
  assetTypeId?: number;
  assetVariantId?: number;
  isActive?: boolean;
};

export async function getDevices(
  params: GetDevicesParams = {},
): Promise<PagedResultDto<DeviceDto>> {
  const searchParams = new URLSearchParams();

  if (params.page !== undefined) {
    searchParams.set("page", String(params.page));
  }

  if (params.pageSize !== undefined) {
    searchParams.set("pageSize", String(params.pageSize));
  }

  if (params.search) {
    searchParams.set("search", params.search);
  }

  if (params.status) {
    searchParams.set("status", params.status);
  }

  if (params.assetTypeId !== undefined) {
    searchParams.set("assetTypeId", String(params.assetTypeId));
  }

  if (params.assetVariantId !== undefined) {
    searchParams.set("assetVariantId", String(params.assetVariantId));
  }

  if (params.isActive !== undefined) {
    searchParams.set("isActive", String(params.isActive));
  }

  const queryString = searchParams.toString();

  return getJson<PagedResultDto<DeviceDto>>(
    `/api/devices${queryString ? `?${queryString}` : ""}`,
  );
}

export async function createDevice(
  request: CreateDeviceRequest,
): Promise<DeviceDto> {
  return postJson<CreateDeviceRequest, DeviceDto>("/api/devices", request);
}

export async function getDeviceById(id: number): Promise<DeviceDto> {
  return getJson<DeviceDto>(`/api/devices/${id}`);
}

export async function updateDevice(
  id: number,
  request: UpdateDeviceRequest,
): Promise<DeviceDto> {
  return putJson<UpdateDeviceRequest, DeviceDto>(`/api/devices/${id}`, request);
}