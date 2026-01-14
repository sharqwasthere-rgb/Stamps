using Stamps.Shared.Services;
using Microsoft.AspNetCore.Http;

namespace Stamps.Web.Services;

public class WebPreferencesService : IPreferencesService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string SessionKeyPrefix = "pref_";

    public WebPreferencesService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession? Session => _httpContextAccessor.HttpContext?.Session;

    public void Set(string key, string value)
    {
        if (Session != null)
        {
            Session.SetString($"{SessionKeyPrefix}{key}", value);
        }
    }

    public void Set(string key, bool value)
    {
        if (Session != null)
        {
            Session.SetString($"{SessionKeyPrefix}{key}", value.ToString());
        }
    }

    public string Get(string key, string defaultValue)
    {
        if (Session != null)
        {
            return Session.GetString($"{SessionKeyPrefix}{key}") ?? defaultValue;
        }
        return defaultValue;
    }

    public bool Get(string key, bool defaultValue)
    {
        if (Session != null)
        {
            var value = Session.GetString($"{SessionKeyPrefix}{key}");
            if (bool.TryParse(value, out var result))
            {
                return result;
            }
        }
        return defaultValue;
    }

    public void Remove(string key)
    {
        if (Session != null)
        {
            Session.Remove($"{SessionKeyPrefix}{key}");
        }
    }
}
