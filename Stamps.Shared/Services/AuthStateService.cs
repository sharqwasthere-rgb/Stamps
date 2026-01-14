namespace Stamps.Shared.Services;

public class AuthStateService
{
    private readonly IPreferencesService _preferences;
    
    private const string UserIdKey = "user_id";
    private const string UserEmailKey = "user_email";
    private const string UserNameKey = "user_name";
    private const string UserTypeKey = "user_type";
    private const string IsLoggedInKey = "is_logged_in";

    public string? UserId { get; private set; }
    public string? Email { get; private set; }
    public string? FullName { get; private set; }
    public string? UserType { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);
    public bool IsCustomer => UserType == "Client";
    public bool IsStoreOwner => UserType == "StoreOwner";

    public event Action? OnAuthStateChanged;

    public AuthStateService(IPreferencesService preferences)
    {
        _preferences = preferences;
    }

    public void Login(AuthResponse user)
    {
        UserId = user.UserId;
        Email = user.Email;
        FullName = user.FullName;
        UserType = user.UserType;

        // Save to preferences
        _preferences.Set(UserIdKey, user.UserId);
        _preferences.Set(UserEmailKey, user.Email);
        _preferences.Set(UserNameKey, user.FullName);
        _preferences.Set(UserTypeKey, user.UserType);
        _preferences.Set(IsLoggedInKey, true);

        OnAuthStateChanged?.Invoke();
    }

    public void Logout()
    {
        UserId = null;
        Email = null;
        FullName = null;
        UserType = null;

        _preferences.Remove(UserIdKey);
        _preferences.Remove(UserEmailKey);
        _preferences.Remove(UserNameKey);
        _preferences.Remove(UserTypeKey);
        _preferences.Remove(IsLoggedInKey);

        OnAuthStateChanged?.Invoke();
    }

    public void LoadFromStorage()
    {
        if (_preferences.Get(IsLoggedInKey, false))
        {
            UserId = _preferences.Get(UserIdKey, string.Empty);
            Email = _preferences.Get(UserEmailKey, string.Empty);
            FullName = _preferences.Get(UserNameKey, string.Empty);
            UserType = _preferences.Get(UserTypeKey, string.Empty);
        }
    }
}
