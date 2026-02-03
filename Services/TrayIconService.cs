using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using Trading212Stick.Helpers;
using Trading212Stick.Views;
using Application = System.Windows.Application;

namespace Trading212Stick.Services
{
    public class TrayIconService : IDisposable
    {
        private NotifyIcon? _notifyIcon;
        private readonly List<StickyNoteWindow> _noteWindows;
        private readonly ConfigurationService _configService;

        public Action? ThemeChanged { get; set; }

        public TrayIconService(ConfigurationService configService)
        {
            _configService = configService;
            _noteWindows = new List<StickyNoteWindow>();
            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            // Laad het custom icon (rond)
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "play_store_512.png");
            var customIcon = IconHelper.CreateRoundIconFromPng(iconPath, 32);

            _notifyIcon = new NotifyIcon
            {
                Icon = customIcon,
                Visible = true,
                Text = "Trading 212 Sticky Notes - Portfolio Monitor"
            };

            UpdateContextMenu();
            _notifyIcon.DoubleClick += (s, e) => ShowAllNotes();
        }

        private void UpdateContextMenu()
        {
            // Dispose old context menu if it exists
            if (_notifyIcon?.ContextMenuStrip != null)
            {
                var oldMenu = _notifyIcon.ContextMenuStrip;
                _notifyIcon.ContextMenuStrip = null;
                oldMenu.Dispose();
            }

            var contextMenu = new ContextMenuStrip();
            
            // Header item (non-clickable)
            var headerItem = new ToolStripMenuItem("📊 Trading 212 Monitor")
            {
                Enabled = false,
                Font = new System.Drawing.Font(contextMenu.Font, System.Drawing.FontStyle.Bold)
            };
            contextMenu.Items.Add(headerItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            
            // Notes visibility management
            contextMenu.Items.Add("👁 Toon Alle Notes", null, (s, e) => ShowAllNotes());
            contextMenu.Items.Add("🚫 Verberg Alle Notes", null, (s, e) => HideAllNotes());
            contextMenu.Items.Add(new ToolStripSeparator());
            
            // Note controls submenu
            var noteControlsMenu = new ToolStripMenuItem("📌 Note Controle");
            noteControlsMenu.DropDownItems.Add("🔓 Unlock Alle Notes", null, (s, e) => UnlockAllNotes());
            noteControlsMenu.DropDownItems.Add("🔒 Lock Alle Notes", null, (s, e) => LockAllNotes());
            noteControlsMenu.DropDownItems.Add(new ToolStripSeparator());
            noteControlsMenu.DropDownItems.Add("📍 Unpin Alle Notes", null, (s, e) => UnpinAllNotes());
            noteControlsMenu.DropDownItems.Add("📌 Pin Alle Notes", null, (s, e) => PinAllNotes());
            contextMenu.Items.Add(noteControlsMenu);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            
            // Theme submenu
            var themeMenu = new ToolStripMenuItem("🎨 Thema");
            var darkThemeItem = new ToolStripMenuItem("🌙 Donker", null, (s, e) => SetTheme("Dark"));
            var lightThemeItem = new ToolStripMenuItem("☀ Licht", null, (s, e) => SetTheme("Light"));
            
            // Set current theme checked
            var currentTheme = _configService.Settings.WindowSettings.Theme;
            darkThemeItem.Checked = currentTheme == "Dark";
            lightThemeItem.Checked = currentTheme == "Light";
            
            themeMenu.DropDownItems.Add(darkThemeItem);
            themeMenu.DropDownItems.Add(lightThemeItem);
            contextMenu.Items.Add(themeMenu);
            
            // Auto-start toggle
            var startupItem = new ToolStripMenuItem("🚀 Start met Windows")
            {
                Checked = IsStartupEnabled(),
                CheckOnClick = true
            };
            startupItem.Click += (s, e) => ToggleStartup(startupItem.Checked);
            contextMenu.Items.Add(startupItem);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            
            // Configuration submenu
            var configMenu = new ToolStripMenuItem("⚙ Configuratie");
            configMenu.DropDownItems.Add("📝 API Instellingen", null, (s, e) => ShowApiSettings());
            configMenu.DropDownItems.Add("📁 Open Configuratiemap", null, (s, e) => OpenConfigFolder());
            configMenu.DropDownItems.Add("🔄 Herlaad Configuratie", null, (s, e) => ReloadConfiguration());
            contextMenu.Items.Add(configMenu);
            
            // Help/About submenu
            var helpMenu = new ToolStripMenuItem("❓ Help");
            helpMenu.DropDownItems.Add("ℹ Over", null, (s, e) => ShowAbout());
            helpMenu.DropDownItems.Add("📖 Documentatie", null, (s, e) => OpenDocumentation());
            contextMenu.Items.Add(helpMenu);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            
            // Exit
            var exitItem = new ToolStripMenuItem("❌ Afsluiten")
            {
                Font = new System.Drawing.Font(contextMenu.Font, System.Drawing.FontStyle.Bold)
            };
            exitItem.Click += (s, e) => ExitApplication();
            contextMenu.Items.Add(exitItem);

            if (_notifyIcon != null)
            {
                _notifyIcon.ContextMenuStrip = contextMenu;
            }
        }

        public void RegisterNoteWindow(StickyNoteWindow window)
        {
            if (!_noteWindows.Contains(window))
            {
                _noteWindows.Add(window);
            }
        }

        private void ShowAllNotes()
        {
            foreach (var window in _noteWindows)
            {
                window.Show();
                window.WindowState = WindowState.Normal;
            }
        }

        private void HideAllNotes()
        {
            foreach (var window in _noteWindows)
            {
                window.Hide();
            }
        }

        private void UnlockAllNotes()
        {
            foreach (var window in _noteWindows)
            {
                if (window.DataContext is ViewModels.StickyNoteViewModel vm)
                {
                    vm.IsLocked = false;
                }
            }
        }

        private void LockAllNotes()
        {
            foreach (var window in _noteWindows)
            {
                if (window.DataContext is ViewModels.StickyNoteViewModel vm)
                {
                    vm.IsLocked = true;
                }
            }
        }

        private void UnpinAllNotes()
        {
            foreach (var window in _noteWindows)
            {
                if (window.DataContext is ViewModels.StickyNoteViewModel vm)
                {
                    vm.IsPinned = false;
                }
            }
        }

        private void PinAllNotes()
        {
            foreach (var window in _noteWindows)
            {
                if (window.DataContext is ViewModels.StickyNoteViewModel vm)
                {
                    vm.IsPinned = true;
                }
            }
        }

        private void SetTheme(string theme)
        {
            _configService.Settings.WindowSettings.Theme = theme;
            _configService.SaveSettings();

            Application.Current.Resources.MergedDictionaries.Clear();
            var themeUri = new Uri($"Themes/{theme}Theme.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary { Source = themeUri }
            );

            ThemeChanged?.Invoke();
            
            // Refresh the tray menu to update checkmarks (without creating new icon)
            UpdateContextMenu();
        }

        private void ShowApiSettings()
        {
            var apiKey = _configService.Settings.Trading212.ApiKey;
            var apiSecret = _configService.Settings.Trading212.ApiSecret;
            var maskedKey = string.IsNullOrEmpty(apiKey) ? "[Niet geconfigureerd]" : $"{apiKey.Substring(0, Math.Min(8, apiKey.Length))}...";
            var maskedSecret = string.IsNullOrEmpty(apiSecret) ? "[Niet geconfigureerd]" : "***************";

            System.Windows.MessageBox.Show(
                $"📊 TRADING 212 API CONFIGURATIE\n\n" +
                $"API URL:\n{_configService.Settings.Trading212.ApiUrl}\n\n" +
                $"API Key: {maskedKey}\n" +
                $"API Secret: {maskedSecret}\n\n" +
                $"Update Interval: {_configService.Settings.Trading212.UpdateIntervalSeconds} seconden\n\n" +
                $"💡 Tip: Gebruik de ⚙ knop in een note voor gedetailleerde instellingen.",
                "API Instellingen",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void OpenConfigFolder()
        {
            var configPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Trading212Stick"
            );

            try
            {
                if (System.IO.Directory.Exists(configPath))
                {
                    System.Diagnostics.Process.Start("explorer.exe", configPath);
                }
                else
                {
                    System.Windows.MessageBox.Show(
                        $"Configuratiemap bestaat nog niet:\n{configPath}\n\nStart de applicatie om de map aan te maken.",
                        "Map niet gevonden",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Kon configuratiemap niet openen:\n{ex.Message}",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void ReloadConfiguration()
        {
            try
            {
                var configPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Trading212Stick",
                    "settings.json"
                );

                if (System.IO.File.Exists(configPath))
                {
                    // Reload settings
                    var json = System.IO.File.ReadAllText(configPath);
                    var newSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.AppSettings>(json);
                    
                    if (newSettings != null)
                    {
                        // Update current settings
                        _configService.Settings.Trading212.ApiUrl = newSettings.Trading212.ApiUrl;
                        _configService.Settings.Trading212.ApiKey = newSettings.Trading212.ApiKey;
                        _configService.Settings.Trading212.ApiSecret = newSettings.Trading212.ApiSecret;
                        _configService.Settings.Trading212.UpdateIntervalSeconds = newSettings.Trading212.UpdateIntervalSeconds;
                        
                        System.Windows.MessageBox.Show(
                            "✅ Configuratie succesvol herladen!\n\nOpmerking: Herstart de applicatie voor update interval wijzigingen.",
                            "Configuratie Herladen",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show(
                        "Configuratiebestand niet gevonden.",
                        "Fout",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Kon configuratie niet herladen:\n{ex.Message}",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void ShowAbout()
        {
            System.Windows.MessageBox.Show(
                "📊 TRADING 212 STICKY NOTES\n\n" +
                "Versie: 1.0.0\n" +
                "Framework: .NET 8.0 WPF\n" +
                "Design: Proton-geïnspireerd\n\n" +
                "✨ FEATURES:\n" +
                "• Realtime portfolio monitoring\n" +
                "• Always-on-top sticky notes\n" +
                "• Dark/Light thema support\n" +
                "• Drag & drop positioning\n" +
                "• Pin/Lock controls\n" +
                "• Configureerbare API settings\n\n" +
                "📝 Een moderne desktop applicatie voor\n" +
                "het monitoren van je Trading 212 portfolio.\n\n" +
                "© 2026 - MIT License",
                "Over Trading 212 Sticky Notes",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void OpenDocumentation()
        {
            var projectPath = AppDomain.CurrentDomain.BaseDirectory;
            var readmePath = System.IO.Path.Combine(projectPath, "README.md");

            try
            {
                if (System.IO.File.Exists(readmePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = readmePath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    System.Windows.MessageBox.Show(
                        "Documentatie niet gevonden.\n\n" +
                        "Bekijk de README.md in de project folder voor volledige documentatie.",
                        "Documentatie",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
            catch
            {
                System.Windows.MessageBox.Show(
                    "Kon documentatie niet openen.\n\n" +
                    "Bekijk de README.md in de project folder:\n" + projectPath,
                    "Documentatie",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        private bool IsStartupEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
                return key?.GetValue("Trading212Stick") != null;
            }
            catch
            {
                return false;
            }
        }

        private void ToggleStartup(bool enable)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                if (enable)
                {
                    var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    exePath = exePath.Replace(".dll", ".exe");
                    key?.SetValue("Trading212Stick", $"\"{exePath}\"");
                }
                else
                {
                    key?.DeleteValue("Trading212Stick", false);
                }

                _configService.Settings.WindowSettings.StartWithWindows = enable;
                _configService.SaveSettings();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Kon autostart instelling niet wijzigen: {ex.Message}",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void ExitApplication()
        {
            Dispose();
            Application.Current.Shutdown();
        }

        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                
                // Dispose context menu first
                if (_notifyIcon.ContextMenuStrip != null)
                {
                    _notifyIcon.ContextMenuStrip.Dispose();
                    _notifyIcon.ContextMenuStrip = null;
                }
                
                // Dispose custom icon
                var icon = _notifyIcon.Icon;
                _notifyIcon.Icon = null;
                
                _notifyIcon.Dispose();
                _notifyIcon = null;
                
                // Dispose icon after NotifyIcon
                icon?.Dispose();
            }
        }
    }
}
