namespace Stamps.Shared.Services;

public interface IScannerService
{
    Task OpenScannerAsync(int cardTypeId, string cardTypeName, int stampsToAdd, int storeId, string storeOwnerId);
}

