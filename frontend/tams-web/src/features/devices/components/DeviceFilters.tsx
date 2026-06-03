import type { AssetTypeDto, AssetVariantDto } from "../types/assetTypeTypes";

export type DeviceFiltersValue = {
  search: string;
  status: string;
  assetTypeId: string;
  assetVariantId: string;
};

type DeviceFiltersProps = {
  value: DeviceFiltersValue;
  assetTypes: AssetTypeDto[];
  onChange: (value: DeviceFiltersValue) => void;
};

export function DeviceFilters({
  value,
  assetTypes,
  onChange,
}: DeviceFiltersProps) {
  const selectedAssetType = assetTypes.find(
    (assetType) => String(assetType.id) === value.assetTypeId,
  );

  const availableVariants: AssetVariantDto[] = selectedAssetType?.variants ?? [];

  function updateFilter<K extends keyof DeviceFiltersValue>(
    key: K,
    nextValue: DeviceFiltersValue[K],
  ) {
    if (key === "assetTypeId") {
      onChange({
        ...value,
        assetTypeId: nextValue,
        assetVariantId: "",
      });

      return;
    }

    onChange({
      ...value,
      [key]: nextValue,
    });
  }

  return (
    <div className="filters-card">
      <div className="filter-field">
        <label htmlFor="device-search">Search</label>
        <input
          id="device-search"
          value={value.search}
          placeholder="Name, UID, barcode..."
          onChange={(event) => updateFilter("search", event.target.value)}
        />
      </div>

      <div className="filter-field">
        <label htmlFor="device-status">Status</label>
        <select
          id="device-status"
          value={value.status}
          onChange={(event) => updateFilter("status", event.target.value)}
        >
          <option value="">All</option>
          <option value="Available">Available</option>
          <option value="Assigned">Assigned</option>
          <option value="Lost">Lost</option>
          <option value="Annulled">Annulled</option>
        </select>
      </div>

      <div className="filter-field">
        <label htmlFor="device-asset-type">Asset type</label>
        <select
          id="device-asset-type"
          value={value.assetTypeId}
          onChange={(event) => updateFilter("assetTypeId", event.target.value)}
        >
          <option value="">All</option>
          {assetTypes.map((assetType) => (
            <option key={assetType.id} value={assetType.id}>
              {assetType.name}
            </option>
          ))}
        </select>
      </div>

      <div className="filter-field">
        <label htmlFor="device-asset-variant">Variant</label>
        <select
          id="device-asset-variant"
          value={value.assetVariantId}
          disabled={!value.assetTypeId}
          onChange={(event) =>
            updateFilter("assetVariantId", event.target.value)
          }
        >
          <option value="">All</option>
          {availableVariants.map((variant) => (
            <option key={variant.id} value={variant.id}>
              {variant.name}
            </option>
          ))}
        </select>
      </div>
    </div>
  );
}