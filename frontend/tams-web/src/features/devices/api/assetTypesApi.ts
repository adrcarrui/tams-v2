import { getJson } from "../../../api/httpClient";
import type { AssetTypeDto } from "../types/assetTypeTypes";

export async function getAssetTypes(): Promise<AssetTypeDto[]> {
  return getJson<AssetTypeDto[]>("/api/asset-types");
}