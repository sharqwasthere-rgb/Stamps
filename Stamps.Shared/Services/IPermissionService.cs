namespace Stamps.Shared.Services;

public interface IPermissionService
{
    Task<PermissionStatus> CheckCameraPermissionAsync();
    Task<PermissionStatus> RequestCameraPermissionAsync();
    Task<PermissionStatus> CheckLocationPermissionAsync();
    Task<PermissionStatus> RequestLocationPermissionAsync();
}

public enum PermissionStatus
{
    Unknown,
    Denied,
    Granted,
    Restricted
}

