import type { DeviceDto } from "../types/deviceTypes";

type DevicesTableProps = {
  devices: DeviceDto[];
  onEdit: (device: DeviceDto) => void;
  onMarkLost: (device: DeviceDto) => void;
  onAnnul: (device: DeviceDto) => void;
  onRestore: (device: DeviceDto) => void;
};

export function DevicesTable({
  devices,
  onEdit,
  onMarkLost,
  onAnnul,
  onRestore,
}: DevicesTableProps) {
  if (devices.length === 0) {
    return <p className="empty-state">No devices found.</p>;
  }

  return (
    <div className="table-card">
      <table className="data-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Type</th>
            <th>Variant</th>
            <th>Identifier</th>
            <th>Status</th>
            <th>Department</th>
            <th className="actions-column">Actions</th>
          </tr>
        </thead>

        <tbody>
          {devices.map((device) => (
            <tr key={device.id}>
              <td>{device.name}</td>
              <td>{device.assetTypeName}</td>
              <td>{device.assetVariantName}</td>
              <td>{getIdentifier(device)}</td>
              <td>
                <span className={`status-pill status-${device.status.toLowerCase()}`}>
                  {device.status}
                </span>
              </td>
              <td>{device.managedByDepartmentCode}</td>
              <td className="actions-column">
                <div className="table-actions">
                  <button
                    type="button"
                    className="table-action-button"
                    onClick={() => onEdit(device)}
                    disabled={device.status === "Annulled"}
                  >
                    Edit
                  </button>

                  {device.status !== "Lost" && device.status !== "Annulled" && (
                    <button
                      type="button"
                      className="table-action-button"
                      onClick={() => onMarkLost(device)}
                    >
                      Mark lost
                    </button>
                  )}

                  {device.status !== "Annulled" && (
                    <button
                      type="button"
                      className="table-action-button danger-action"
                      onClick={() => onAnnul(device)}
                    >
                      Annul
                    </button>
                  )}

                  {(device.status === "Lost" || device.status === "Annulled") && (
                    <button
                      type="button"
                      className="table-action-button"
                      onClick={() => onRestore(device)}
                    >
                      Restore
                    </button>
                  )}
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function getIdentifier(device: DeviceDto): string {
  if (device.uid) {
    return `UID: ${device.uid}`;
  }

  if (device.barcode) {
    return `BARCODE: ${device.barcode}`;
  }

  if (device.serialNumber) {
    return `SN: ${device.serialNumber}`;
  }

  return "—";
}