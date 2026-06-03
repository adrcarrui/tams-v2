import { useMemo, useState, type FormEvent } from "react";
import { useMutation } from "@tanstack/react-query";
import { ApiClientError } from "../../../api/httpClient";
import { updateDevice } from "../api/devicesApi";
import type { AssetTypeDto, AssetVariantDto } from "../types/assetTypeTypes";
import type {
  DeviceDto,
  UpdateDeviceRequest,
} from "../types/deviceTypes";

type EditDeviceModalProps = {
  device: DeviceDto;
  assetTypes: AssetTypeDto[];
  onClose: () => void;
  onUpdated: (device: DeviceDto) => void;
};

type FormState = {
  name: string;
  assetTypeId: string;
  assetVariantId: string;
  uid: string;
  barcode: string;
  serialNumber: string;
  notes: string;
  isActive: boolean;
};

export function EditDeviceModal({
  device,
  assetTypes,
  onClose,
  onUpdated,
}: EditDeviceModalProps) {
  const [form, setForm] = useState<FormState>({
    name: device.name,
    assetTypeId: String(device.assetTypeId),
    assetVariantId: String(device.assetVariantId),
    uid: device.uid ?? "",
    barcode: device.barcode ?? "",
    serialNumber: device.serialNumber ?? "",
    notes: device.notes ?? "",
    isActive: device.isActive,
  });

  const [clientError, setClientError] = useState<string | null>(null);

  const selectedAssetType = useMemo(
    () => assetTypes.find((assetType) => String(assetType.id) === form.assetTypeId),
    [assetTypes, form.assetTypeId],
  );

  const availableVariants: AssetVariantDto[] = selectedAssetType?.variants ?? [];

  const selectedVariant = availableVariants.find(
    (variant) => String(variant.id) === form.assetVariantId,
  );

  const mutation = useMutation({
    mutationFn: (request: UpdateDeviceRequest) =>
      updateDevice(device.id, request),
    onSuccess: (updatedDevice) => {
      onUpdated(updatedDevice);
    },
  });

  function updateField<K extends keyof FormState>(key: K, value: FormState[K]) {
    setClientError(null);

    if (key === "assetTypeId") {
      setForm({
        ...form,
        assetTypeId: value,
        assetVariantId: "",
      });

      return;
    }

    setForm({
      ...form,
      [key]: value,
    });
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const validationError = validateForm(form, selectedAssetType);

    if (validationError) {
      setClientError(validationError);
      return;
    }

    const request: UpdateDeviceRequest = {
      name: form.name.trim(),
      assetVariantId: Number(form.assetVariantId),
      uid: normalizeOptionalText(form.uid),
      barcode: normalizeOptionalText(form.barcode),
      serialNumber: normalizeOptionalText(form.serialNumber),
      notes: normalizeOptionalText(form.notes),
      isActive: form.isActive,
    };

    mutation.mutate(request);
  }

  const errorMessage =
    clientError ??
    (mutation.error instanceof ApiClientError
      ? mutation.error.message
      : mutation.error instanceof Error
        ? mutation.error.message
        : null);

  return (
    <div className="modal-backdrop" role="presentation">
      <div className="modal-card" role="dialog" aria-modal="true">
        <div className="modal-header">
          <div>
            <h2>Edit device</h2>
            <p>Update device details and identifiers.</p>
          </div>

          <button type="button" className="icon-button" onClick={onClose}>
            ×
          </button>
        </div>

        {errorMessage && (
          <div className="form-error">
            <strong>Could not update device.</strong>
            <p>{errorMessage}</p>
          </div>
        )}

        <form className="form-grid" onSubmit={handleSubmit}>
          <div className="form-field">
            <label htmlFor="edit-device-name">Name</label>
            <input
              id="edit-device-name"
              value={form.name}
              onChange={(event) => updateField("name", event.target.value)}
            />
          </div>

          <div className="form-field">
            <label htmlFor="edit-device-active">Active</label>
            <select
              id="edit-device-active"
              value={String(form.isActive)}
              onChange={(event) =>
                updateField("isActive", event.target.value === "true")
              }
            >
              <option value="true">Active</option>
              <option value="false">Inactive</option>
            </select>
          </div>

          <div className="form-field">
            <label htmlFor="edit-device-asset-type">Asset type</label>
            <select
              id="edit-device-asset-type"
              value={form.assetTypeId}
              onChange={(event) => updateField("assetTypeId", event.target.value)}
            >
              <option value="">Select type</option>
              {assetTypes.map((assetType) => (
                <option key={assetType.id} value={assetType.id}>
                  {assetType.name}
                </option>
              ))}
            </select>
          </div>

          <div className="form-field">
            <label htmlFor="edit-device-asset-variant">Variant</label>
            <select
              id="edit-device-asset-variant"
              value={form.assetVariantId}
              disabled={!form.assetTypeId}
              onChange={(event) =>
                updateField("assetVariantId", event.target.value)
              }
            >
              <option value="">Select variant</option>
              {availableVariants.map((variant) => (
                <option key={variant.id} value={variant.id}>
                  {variant.name}
                </option>
              ))}
            </select>
          </div>

          <div className="form-field">
            <label htmlFor="edit-device-uid">
              UID {selectedAssetType?.identifierPolicy === "Rfid" ? "*" : ""}
            </label>
            <input
              id="edit-device-uid"
              value={form.uid}
              onChange={(event) => updateField("uid", event.target.value)}
            />
          </div>

          <div className="form-field">
            <label htmlFor="edit-device-barcode">
              Barcode{" "}
              {selectedAssetType?.identifierPolicy === "Barcode" ? "*" : ""}
            </label>
            <input
              id="edit-device-barcode"
              value={form.barcode}
              onChange={(event) => updateField("barcode", event.target.value)}
            />
          </div>

          <div className="form-field">
            <label htmlFor="edit-device-serial-number">Serial number</label>
            <input
              id="edit-device-serial-number"
              value={form.serialNumber}
              onChange={(event) =>
                updateField("serialNumber", event.target.value)
              }
            />
          </div>

          <div className="form-field form-field-full">
            <label htmlFor="edit-device-notes">Notes</label>
            <textarea
              id="edit-device-notes"
              value={form.notes}
              onChange={(event) => updateField("notes", event.target.value)}
              rows={3}
            />
          </div>

          {selectedAssetType && selectedVariant && (
            <div className="form-hint form-field-full">
              <strong>{selectedAssetType.name}</strong> / {selectedVariant.name} ·
              Identifier policy: {selectedAssetType.identifierPolicy}
            </div>
          )}

          {device.status === "Annulled" && (
            <div className="form-error form-field-full">
              <strong>This device is annulled.</strong>
              <p>The backend does not allow editing annulled devices.</p>
            </div>
          )}

          <div className="modal-actions">
            <button type="button" className="secondary-button" onClick={onClose}>
              Cancel
            </button>

            <button
              type="submit"
              className="primary-button"
              disabled={mutation.isPending || device.status === "Annulled"}
            >
              {mutation.isPending ? "Saving..." : "Save changes"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

function validateForm(
  form: FormState,
  selectedAssetType: AssetTypeDto | undefined,
): string | null {
  if (!form.name.trim()) {
    return "Device name is required.";
  }

  if (!form.assetTypeId) {
    return "Asset type is required.";
  }

  if (!form.assetVariantId) {
    return "Asset variant is required.";
  }

  if (!selectedAssetType) {
    return "Selected asset type is invalid.";
  }

  const hasUid = Boolean(form.uid.trim());
  const hasBarcode = Boolean(form.barcode.trim());

  switch (selectedAssetType.identifierPolicy) {
    case "Rfid":
      return hasUid ? null : "UID is required for this asset type.";

    case "Barcode":
      return hasBarcode ? null : "Barcode is required for this asset type.";

    case "RfidOrBarcode":
      return hasUid || hasBarcode
        ? null
        : "UID or barcode is required for this asset type.";

    case "RfidAndBarcode":
      return hasUid && hasBarcode
        ? null
        : "UID and barcode are required for this asset type.";

    default:
      return null;
  }
}

function normalizeOptionalText(value: string): string | null {
  const trimmedValue = value.trim();

  return trimmedValue.length > 0 ? trimmedValue : null;
}