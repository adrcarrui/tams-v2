import { useState } from "react";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { getAssetTypes } from "../api/assetTypesApi";
import { getDevices } from "../api/devicesApi";
import { CreateDeviceModal } from "../components/CreateDeviceModal";
import {
  DeviceFilters,
  type DeviceFiltersValue,
} from "../components/DeviceFilters";
import { DevicesTable } from "../components/DevicesTable";
import { PaginationControls } from "../components/PaginationControls";

const pageSize = 25;

const initialFilters: DeviceFiltersValue = {
  search: "",
  status: "",
  assetTypeId: "",
  assetVariantId: "",
};

export function DevicesPage() {
  const queryClient = useQueryClient();

  const [page, setPage] = useState(1);
  const [filters, setFilters] = useState<DeviceFiltersValue>(initialFilters);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

  const assetTypesQuery = useQuery({
    queryKey: ["asset-types"],
    queryFn: getAssetTypes,
  });

  const devicesQuery = useQuery({
    queryKey: ["devices", { page, pageSize, filters }],
    queryFn: () =>
      getDevices({
        page,
        pageSize,
        search: filters.search || undefined,
        status: filters.status || undefined,
        assetTypeId: filters.assetTypeId
          ? Number(filters.assetTypeId)
          : undefined,
        assetVariantId: filters.assetVariantId
          ? Number(filters.assetVariantId)
          : undefined,
        isActive: true,
      }),
  });

  function handleFiltersChange(nextFilters: DeviceFiltersValue) {
    setFilters(nextFilters);
    setPage(1);
  }

  async function handleDeviceCreated() {
    setIsCreateModalOpen(false);
    setPage(1);

    await queryClient.invalidateQueries({
      queryKey: ["devices"],
    });
  }

  if (assetTypesQuery.isLoading || devicesQuery.isLoading) {
    return <p>Loading devices...</p>;
  }

  if (assetTypesQuery.isError) {
    return (
      <div className="error-box">
        <strong>Could not load asset types.</strong>
        <p>
          {assetTypesQuery.error instanceof Error
            ? assetTypesQuery.error.message
            : "Unknown error"}
        </p>
      </div>
    );
  }

  if (devicesQuery.isError) {
    return (
      <div className="error-box">
        <strong>Could not load devices.</strong>
        <p>
          {devicesQuery.error instanceof Error
            ? devicesQuery.error.message
            : "Unknown error"}
        </p>
      </div>
    );
  }

  const devices = devicesQuery.data;

  return (
    <section>
      <div className="page-header">
        <div>
          <h1>Devices</h1>
          <p>Manage TAMS devices and their operational status.</p>
        </div>

        <button
          type="button"
          className="primary-button"
          onClick={() => setIsCreateModalOpen(true)}
        >
          New device
        </button>
      </div>

      <DeviceFilters
        value={filters}
        assetTypes={assetTypesQuery.data ?? []}
        onChange={handleFiltersChange}
      />

      <DevicesTable devices={devices?.items ?? []} />

      <PaginationControls
        page={devices?.page ?? 1}
        totalPages={devices?.totalPages ?? 0}
        totalItems={devices?.totalItems ?? 0}
        hasPreviousPage={devices?.hasPreviousPage ?? false}
        hasNextPage={devices?.hasNextPage ?? false}
        onPageChange={setPage}
      />

      {isCreateModalOpen && (
        <CreateDeviceModal
          assetTypes={assetTypesQuery.data ?? []}
          onClose={() => setIsCreateModalOpen(false)}
          onCreated={handleDeviceCreated}
        />
      )}
    </section>
  );
}