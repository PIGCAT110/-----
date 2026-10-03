using PowerManager.Core.Configuration;
using Xunit;

namespace PowerManager.Core.Tests.Configuration;

public class SettingsStoreTests : IDisposable
{
    private readonly string _dir;

    public SettingsStoreTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "pm-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
    }

    [Fact]
    public void Load_returns_defaults_when_file_missing()
    {
        var store = new SettingsStore(Path.Combine(_dir, "missing.json"));

        var settings = store.Load();

        Assert.True(settings.MinimizeToTrayOnClose);
    }

    [Fact]
    public void Save_then_load_roundtrips_values()
    {
        var path = Path.Combine(_dir, "settings.json");
        var store = new SettingsStore(path);

        store.Save(new AppSettings { MinimizeToTrayOnClose = false });
        var loaded = store.Load();

        Assert.False(loaded.MinimizeToTrayOnClose);
    }

    [Fact]
    public void Save_creates_missing_parent_directory()
    {
        var path = Path.Combine(_dir, "nested", "deep", "settings.json");
        var store = new SettingsStore(path);

        store.Save(new AppSettings());

        Assert.True(File.Exists(path));
    }

    [Fact]
    public void Load_falls_back_to_defaults_when_json_corrupt()
    {
        var path = Path.Combine(_dir, "corrupt.json");
        File.WriteAllText(path, "{ this is not valid json ");
        var store = new SettingsStore(path);

        var settings = store.Load();

        Assert.True(settings.MinimizeToTrayOnClose);
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(_dir, recursive: true);
        }
        catch (DirectoryNotFoundException)
        {
        }
    }
}
