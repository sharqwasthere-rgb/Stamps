using Stamps.Shared.Services;

namespace Stamps.Services;

public class PreferencesService : IPreferencesService
{
    public void Set(string key, string value) => Preferences.Set(key, value);
    public void Set(string key, bool value) => Preferences.Set(key, value);
    public string Get(string key, string defaultValue) => Preferences.Get(key, defaultValue);
    public bool Get(string key, bool defaultValue) => Preferences.Get(key, defaultValue);
    public void Remove(string key) => Preferences.Remove(key);
}

