using System.Windows;
using PowerManager.App.Infrastructure;
using PowerManager.Core.Configuration;

namespace PowerManager.App.Views;

public partial class SettingsWindow : Window
{
    private readonly AppSettings _settings;
    private readonly SettingsStore _settingsStore;

    public SettingsWindow(AppSettings settings, SettingsStore settingsStore)
    {
        InitializeComponent();

        _settings = settings;
        _settingsStore = settingsStore;

        StartupCheckBox.IsChecked = WindowsStartup.IsEnabled();
        MinimizeToTrayCheckBox.IsChecked = settings.MinimizeToTrayOnClose;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        WindowsStartup.SetEnabled(StartupCheckBox.IsChecked == true);

        _settings.MinimizeToTrayOnClose = MinimizeToTrayCheckBox.IsChecked == true;
        _settingsStore.Save(_settings);

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
