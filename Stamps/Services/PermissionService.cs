using Stamps.Shared.Services;
using PermissionStatusShared = Stamps.Shared.Services.PermissionStatus;

namespace Stamps.Services;

public class PermissionService : IPermissionService
{
    public async Task<PermissionStatusShared> CheckCameraPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        return ConvertStatus(status);
    }

    public async Task<PermissionStatusShared> RequestCameraPermissionAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.Camera>();
        return ConvertStatus(status);
    }

    public async Task<PermissionStatusShared> CheckLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        return ConvertStatus(status);
    }

    public async Task<PermissionStatusShared> RequestLocationPermissionAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        return ConvertStatus(status);
    }

    private PermissionStatusShared ConvertStatus(Microsoft.Maui.ApplicationModel.PermissionStatus status)
    {
        return status switch
        {
            Microsoft.Maui.ApplicationModel.PermissionStatus.Granted => PermissionStatusShared.Granted,
            Microsoft.Maui.ApplicationModel.PermissionStatus.Denied => PermissionStatusShared.Denied,
            Microsoft.Maui.ApplicationModel.PermissionStatus.Restricted => PermissionStatusShared.Restricted,
            _ => PermissionStatusShared.Unknown
        };
    }
}

