using System.Windows;
using Trading212Stick.Services;
using Trading212Stick.ViewModels;
using Trading212Stick.Views;

namespace Trading212Stick
{
    public partial class App : Application
    {
        private ConfigurationService? _configService;
        private Trading212Service? _tradingService;
        private TrayIconService? _trayService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Initialize services
                _configService = new ConfigurationService();
                _tradingService = new Trading212Service(_configService);
                // Load theme
                LoadTheme(_configService.Settings.WindowSettings.Theme);

                // Create main view model
                var mainViewModel = new MainViewModel(_configService, _tradingService);

                // Initialize tray service (requires configuration)
                _trayService = new TrayIconService(_configService);
                _trayService.ThemeChanged = () => mainViewModel.OnThemeChanged();

                // Create sticky note windows for each note in the view model
                foreach (var noteViewModel in mainViewModel.Notes)
                {
                    var noteWindow = new StickyNoteWindow(noteViewModel, _configService);
                    _trayService.RegisterNoteWindow(noteWindow);
                    noteWindow.Show();
                }

                // Don't create a main window - we only have sticky notes and tray icon
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }
            catch (System.Exception ex)
            {
                try
                {
                    var appData = System.IO.Path.Combine(
                        System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                        "Trading212Stick"
                    );
                    System.IO.Directory.CreateDirectory(appData);
                    var logPath = System.IO.Path.Combine(appData, "startup_error.log");
                    System.IO.File.WriteAllText(logPath, ex.ToString());
                }
                catch { }

                System.Windows.MessageBox.Show($"Startup error: {ex.Message}\nSee %appdata%\\Trading212Stick\\startup_error.log for details.", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void LoadTheme(string theme)
        {
            Resources.MergedDictionaries.Clear();
            var themeUri = new System.Uri($"Themes/{theme}Theme.xaml", System.UriKind.Relative);
            Resources.MergedDictionaries.Add(new ResourceDictionary { Source = themeUri });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayService?.Dispose();
            base.OnExit(e);
        }
    }
}
