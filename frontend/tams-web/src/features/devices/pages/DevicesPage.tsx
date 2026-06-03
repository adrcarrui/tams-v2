import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getAssetTypes } from "../api/assetTypesApi";
import {
    annulDevice,
    getDevices,
    markDeviceAsLost,
    restoreDevice,
} from "../api/devicesApi";
import { CreateDeviceModal } from "../components/CreateDeviceModal";
import {
    DeviceFilters,
    type DeviceFiltersValue,
} from "../components/DeviceFilters";
import { DevicesTable } from "../components/DevicesTable";
import { EditDeviceModal } from "../components/EditDeviceModal";
import { PaginationControls } from "../components/PaginationControls";
import type { DeviceDto } from "../types/deviceTypes";

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
    const [editingDevice, setEditingDevice] = useState<DeviceDto | null>(null);
    const [statusActionError, setStatusActionError] = useState<string | null>(null);

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

    const markLostMutation = useMutation({
        mutationFn: markDeviceAsLost,
        onSuccess: async () => {
            setStatusActionError(null);
            await refreshDevices();
        },
        onError: (error) => {
            setStatusActionError(
                error instanceof Error ? error.message : "Could not mark device as lost.",
            );
        },
    });

    const annulMutation = useMutation({
        mutationFn: annulDevice,
        onSuccess: async () => {
            setStatusActionError(null);
            await refreshDevices();
        },
        onError: (error) => {
            setStatusActionError(
                error instanceof Error ? error.message : "Could not annul device.",
            );
        },
    });

    const restoreMutation = useMutation({
        mutationFn: restoreDevice,
        onSuccess: async () => {
            setStatusActionError(null);
            await refreshDevices();
        },
        onError: (error) => {
            setStatusActionError(
                error instanceof Error ? error.message : "Could not restore device.",
            );
        },
    });

    function handleMarkLost(device: DeviceDto) {
        markLostMutation.mutate(device.id);
    }

    function handleAnnul(device: DeviceDto) {
        const confirmed = window.confirm(
            `Are you sure you want to annul "${device.name}"? This will deactivate the device.`,
        );

        if (!confirmed) {
            return;
        }

        annulMutation.mutate(device.id);
    }

    function handleRestore(device: DeviceDto) {
        restoreMutation.mutate(device.id);
    }

    function handleFiltersChange(nextFilters: DeviceFiltersValue) {
        setFilters(nextFilters);
        setPage(1);
    }

    async function refreshDevices() {
        await queryClient.invalidateQueries({
            queryKey: ["devices"],
        });
    }

    async function handleDeviceCreated() {
        setIsCreateModalOpen(false);
        setPage(1);
        await refreshDevices();
    }

    async function handleDeviceUpdated() {
        setEditingDevice(null);
        await refreshDevices();
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
    const assetTypes = assetTypesQuery.data ?? [];

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

            {statusActionError && (
                <div className="error-box">
                    <strong>Could not update device status.</strong>
                    <p>{statusActionError}</p>
                </div>
            )}

            <DeviceFilters
                value={filters}
                assetTypes={assetTypes}
                onChange={handleFiltersChange}
            />

            <DevicesTable
                devices={devices?.items ?? []}
                onEdit={setEditingDevice}
                onMarkLost={handleMarkLost}
                onAnnul={handleAnnul}
                onRestore={handleRestore}
            />

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
                    assetTypes={assetTypes}
                    onClose={() => setIsCreateModalOpen(false)}
                    onCreated={handleDeviceCreated}
                />
            )}

            {editingDevice && (
                <EditDeviceModal
                    device={editingDevice}
                    assetTypes={assetTypes}
                    onClose={() => setEditingDevice(null)}
                    onUpdated={handleDeviceUpdated}
                />
            )}
        </section>
    );
}