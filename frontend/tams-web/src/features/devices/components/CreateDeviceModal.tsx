import { useMemo, useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { ApiClientError } from "../../../api/httpClient";
import { createDevice } from "../api/devicesApi";
import type { AssetTypeDto, AssetVariantDto } from "../types/assetTypeTypes";
import type { CreateDeviceRequest, DeviceDto } from "../types/deviceTypes";

type CreateDeviceModalProps = {
  assetTypes: AssetTypeDto[];
  onClose: () => void;
  onCreated: (device: DeviceDto) => void;
};

type FormState = {
  name: string;
  assetTypeId: string;
  assetVariantId: string;
  uid: string;
  barcode: string;
  serialNumber: string;
  notes: string;
};

const initialFormState: FormState = {
  name: "",
  assetTypeId: "",
  assetVariantId: "",
  uid: "",
  barcode: "",
  serialNumber: "",
  notes: "",
};

export function CreateDeviceModal({
  assetTypes,
  onClose,
  onCreated,
}: CreateDeviceModalProps) {
  const [form, setForm] = useState<FormState>(initialFormState);
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
    mutationFn: createDevice,
    onSuccess: (device) => {
      onCreated(device);
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

  function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const validationError = validateForm(form, selectedAssetType);

    if (validationError) {
      setClientError(validationError);
      return;
    }

    const request: CreateDeviceRequest = {
      name: form.name.trim(),
      assetVariantId: Number(form.assetVariantId),
      uid: normalizeOptionalText(form.uid),
      barcode: normalizeOptionalText(form.barcode),
      serialNumber: normalizeOptionalText(form.serialNumber),
      notes: normalizeOptionalText(form.notes),
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
            <h2>New device</h2>
            <p>Create a TAMS device with its type, variant and identifier.</p>
          </div>

          <button type="button" className="icon-button" onClick={onClose}>
            ×
          </button>
        </div>

        {errorMessage && (
          <div className="form-error">
            <strong>Could not create device.</strong>
            <p>{errorMessage}</p>
          </div>
        )}

        <form className="form-grid" onSubmit={handleSubmit}>
          <div className="form-field">
            <label htmlFor="device-name">Name</label>
            <input
              id="device-name"
              value={form.name}
              onChange={(event) => updateField("name", event.target.value)}
              placeholder="Card Vending 001"
            />
          </div>

          <div className="form-field">
            <label htmlFor="device-asset-type">Asset type</label>
            <select
              id="device-asset-type"
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
            <label htmlFor="device-asset-variant">Variant</label>
            <select
              id="device-asset-variant"
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
            <label htmlFor="device-uid">
              UID {selectedAssetType?.identifierPolicy === "Rfid" ? "*" : ""}
            </label>
            <input
              id="device-uid"
              value={form.uid}
              onChange={(event) => updateField("uid", event.target.value)}
              placeholder="04AABBCC01"
            />
          </div>

          <div className="form-field">
            <label htmlFor="device-barcode">
              Barcode{" "}
              {selectedAssetType?.identifierPolicy === "Barcode" ? "*" : ""}
            </label>
            <input
              id="device-barcode"
              value={form.barcode}
              onChange={(event) => updateField("barcode", event.target.value)}
              placeholder="LAP-G10-001"
            />
          </div>

          <div className="form-field">
            <label htmlFor="device-serial-number">Serial number</label>
            <input
              id="device-serial-number"
              value={form.serialNumber}
              onChange={(event) =>
                updateField("serialNumber", event.target.value)
              }
              placeholder="SN-G10-001"
            />
          </div>

          <div className="form-field form-field-full">
            <label htmlFor="device-notes">Notes</label>
            <textarea
              id="device-notes"
              value={form.notes}
              onChange={(event) => updateField("notes", event.target.value)}
              placeholder="Optional notes"
              rows={3}
            />
          </div>

          {selectedAssetType && selectedVariant && (
            <div className="form-hint form-field-full">
              <strong>{selectedAssetType.name}</strong> / {selectedVariant.name} ·
              Identifier policy: {selectedAssetType.identifierPolicy}
            </div>
          )}

          <div className="modal-actions">
            <button type="button" className="secondary-button" onClick={onClose}>
              Cancel
            </button>

            <button
              type="submit"
              className="primary-button"
              disabled={mutation.isPending}
            >
              {mutation.isPending ? "Creating..." : "Create device"}
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