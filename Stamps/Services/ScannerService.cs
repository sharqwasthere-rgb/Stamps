using Stamps.Pages;
using Stamps.Shared.Services;

namespace Stamps.Services;

public class ScannerService : IScannerService
{
    public async Task OpenScannerAsync(int cardTypeId, string cardTypeName, int stampsToAdd, int storeId, string storeOwnerId)
    {
        var scannerPage = new ScannerPage(cardTypeId, cardTypeName, stampsToAdd, storeId, storeOwnerId);
        
        if (Application.Current?.Windows.Count > 0)
        {
            var page = Application.Current.Windows[0].Page;
            if (page != null)
            {
                await page.Navigation.PushModalAsync(scannerPage);
            }
        }
    }
}
