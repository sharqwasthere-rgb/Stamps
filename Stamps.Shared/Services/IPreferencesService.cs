namespace Stamps.Shared.Services;

public interface IPreferencesService
{
    void Set(string key, string value);
    void Set(string key, bool value);
    string Get(string key, string defaultValue);
    bool Get(string key, bool defaultValue);
    void Remove(string key);
}

