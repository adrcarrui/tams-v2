namespace Tams.Api.Domain.Enums;

public enum AssetIdentifierPolicy
{
    None = 0,
    Rfid = 1,
    Barcode = 2,
    RfidOrBarcode = 3,
    RfidAndBarcode = 4,
    NFC = 5
}