using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Trading212Stick.Services;

namespace Trading212Stick.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ConfigurationService _configService;
        private readonly Trading212Service _trading212Service;
        private readonly DispatcherTimer _updateTimer;
        private string _currentTheme;

        public MainViewModel(ConfigurationService configService, Trading212Service trading212Service)
        {
            _configService = configService;
            _trading212Service = trading212Service;
            _currentTheme = _configService.Settings.WindowSettings.Theme;

            Notes = new ObservableCollection<StickyNoteViewModel>();
            
            // Maak standaard portfolio note
            var portfolioNote = new StickyNoteViewModel("portfolio_main", "Trading 212 Portfolio", _configService);
            Notes.Add(portfolioNote);

            // Setup update timer
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(_configService.Settings.Trading212.UpdateIntervalSeconds)
            };
            _updateTimer.Tick += async (s, e) => await UpdateAllNotesAsync();
            _updateTimer.Start();

            // Commands
            ToggleThemeCommand = new RelayCommand(ToggleTheme);
            ExitCommand = new RelayCommand(Exit);
            ShowSettingsCommand = new RelayCommand(ShowSettings);

            // Initial update
            _ = UpdateAllNotesAsync();
        }

        public ObservableCollection<StickyNoteViewModel> Notes { get; }

        public string CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (SetProperty(ref _currentTheme, value))
                {
                    _configService.Settings.WindowSettings.Theme = value;
                    _configService.SaveSettings();
                }
            }
        }

        public ICommand ToggleThemeCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ShowSettingsCommand { get; }

        private async System.Threading.Tasks.Task UpdateAllNotesAsync()
        {
            foreach (var note in Notes)
            {
                note.IsLoading = true;
            }

            var portfolioData = await _trading212Service.GetPortfolioDataAsync();

            foreach (var note in Notes)
            {
                note.UpdateFromPortfolio(portfolioData);
                note.IsLoading = false;
            }
        }

        private void ToggleTheme()
        {
            CurrentTheme = CurrentTheme == "Dark" ? "Light" : "Dark";
            
            // Update thema in Application Resources
            Application.Current.Resources.MergedDictionaries.Clear();
            var themeUri = new Uri($"Themes/{CurrentTheme}Theme.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary { Source = themeUri }
            );
        }

        private void ShowSettings()
        {
            MessageBox.Show(
                $"Configuratie bestand: {System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trading212Stick", "settings.json")}\n\n" +
                $"API URL: {_configService.Settings.Trading212.ApiUrl}\n" +
                $"Update interval: {_configService.Settings.Trading212.UpdateIntervalSeconds} seconden",
                "Instellingen",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void Exit()
        {
            _updateTimer.Stop();
            Application.Current.Shutdown();
        }

        public void OnThemeChanged()
        {
            foreach (var note in Notes)
            {
                note.RaisePropertyChangedForColors();
            }
        }
    }
}
