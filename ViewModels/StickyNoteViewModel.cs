using System;
using System.Windows.Input;
using Trading212Stick.Models;
using Trading212Stick.Services;

namespace Trading212Stick.ViewModels
{
    public class StickyNoteViewModel : ViewModelBase
    {
        private readonly ConfigurationService _configService;
        private string _noteId;
        private string _title;
        private bool _isPinned; // Default false -> when true: not movable
        private bool _isLocked; // when true: stays on foreground (Topmost)
        private bool _isSettingsVisible;
        
        // Settings Edit Fields
        private string _editApiUrl = string.Empty;
        private string _editApiKey = string.Empty;
        private string _editApiSecret = string.Empty;
        private int _editUpdateInterval = 60;

        private decimal _totalValue;
        private decimal _dayResult;
        private decimal _dayResultPercentage;
        private decimal _totalProfitLoss;
        private decimal _totalProfitLossPercentage;
        private string _currency;
        private DateTime _lastUpdated;
        private bool _isError;
        private string? _errorMessage;
        private bool _isLoading;

        public StickyNoteViewModel(string noteId, string title, ConfigurationService configService)
        {
            _noteId = noteId;
            _title = title;
            _configService = configService;
            _currency = "EUR";
            _lastUpdated = DateTime.Now;

            // Initialize edit fields from current config
            _editApiUrl = _configService.Settings.Trading212.ApiUrl ?? string.Empty;
            _editApiKey = _configService.Settings.Trading212.ApiKey ?? string.Empty;
            _editApiSecret = _configService.Settings.Trading212.ApiSecret ?? string.Empty;
            _editUpdateInterval = _configService.Settings.Trading212.UpdateIntervalSeconds;

            TogglePinCommand = new RelayCommand(TogglePin);
            ToggleLockCommand = new RelayCommand(ToggleLock);
            ToggleSettingsCommand = new RelayCommand(ToggleSettings);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        public string NoteId
        {
            get => _noteId;
            set => SetProperty(ref _noteId, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public bool IsPinned
        {
            get => _isPinned;
            set
            {
                if (SetProperty(ref _isPinned, value))
                {
                    OnPropertyChanged(nameof(PinButtonOpacity));
                }
            }
        }

        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (SetProperty(ref _isLocked, value))
                {
                    OnPropertyChanged(nameof(IsTopmost));
                }
            }
        }
        
        public bool IsSettingsVisible
        {
            get => _isSettingsVisible;
            set => SetProperty(ref _isSettingsVisible, value);
        }

        // Settings Properties
        public string EditApiUrl
        {
            get => _editApiUrl;
            set => SetProperty(ref _editApiUrl, value);
        }

        public string EditApiKey
        {
            get => _editApiKey;
            set => SetProperty(ref _editApiKey, value);
        }

        public string EditApiSecret
        {
            get => _editApiSecret;
            set => SetProperty(ref _editApiSecret, value);
        }

        public int EditUpdateInterval
        {
            get => _editUpdateInterval;
            set => SetProperty(ref _editUpdateInterval, value);
        }


        // When Locked is TRUE, window stays Topmost (foreground)
        public bool IsTopmost => IsLocked;

        // When Pinned is TRUE, window is locked in place (not movable)
        public double PinButtonOpacity => IsPinned ? 1.0 : 0.6;

        public ICommand TogglePinCommand { get; }
        public ICommand ToggleLockCommand { get; }
        public ICommand ToggleSettingsCommand { get; }
        public ICommand SaveSettingsCommand { get; }

        private void TogglePin()
        {
            IsPinned = !IsPinned;
        }

        private void ToggleLock()
        {
            IsLocked = !IsLocked;
            OnPropertyChanged(nameof(IsTopmost));
        }

        private void ToggleSettings()
        {
            if (!IsSettingsVisible)
            {
                // Load current settings into edit fields
                EditApiUrl = _configService.Settings.Trading212.ApiUrl;
                EditApiKey = _configService.Settings.Trading212.ApiKey;
                EditApiSecret = _configService.Settings.Trading212.ApiSecret;
                EditUpdateInterval = _configService.Settings.Trading212.UpdateIntervalSeconds;
            }
            IsSettingsVisible = !IsSettingsVisible;
        }

        private void SaveSettings()
        {
            _configService.Settings.Trading212.ApiUrl = EditApiUrl;
            _configService.Settings.Trading212.ApiKey = EditApiKey;
            _configService.Settings.Trading212.ApiSecret = EditApiSecret;
            _configService.Settings.Trading212.UpdateIntervalSeconds = EditUpdateInterval;
            
            _configService.SaveSettings();
            IsSettingsVisible = false;
            
            // Note: A restart might be needed for interval changes to take effect in MainViewModel timer
            // but we can at least save it. Ideally, MainViewModel listens to config changes.
        }

        public decimal TotalValue
        {
            get => _totalValue;
            set => SetProperty(ref _totalValue, value);
        }

        public decimal DayResult
        {
            get => _dayResult;
            set => SetProperty(ref _dayResult, value);
        }

        public decimal DayResultPercentage
        {
            get => _dayResultPercentage;
            set => SetProperty(ref _dayResultPercentage, value);
        }

        public decimal TotalProfitLoss
        {
            get => _totalProfitLoss;
            set => SetProperty(ref _totalProfitLoss, value);
        }

        public decimal TotalProfitLossPercentage
        {
            get => _totalProfitLossPercentage;
            set => SetProperty(ref _totalProfitLossPercentage, value);
        }

        public string Currency
        {
            get => _currency;
            set => SetProperty(ref _currency, value);
        }

        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
        }

        public bool IsError
        {
            get => _isError;
            set => SetProperty(ref _isError, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string DayResultColor => DayResult >= 0 ? "#4CAF50" : "#F44336";
        public string TotalProfitLossColor => TotalProfitLoss >= 0 ? "#4CAF50" : "#F44336";

        public void UpdateFromPortfolio(PortfolioInfo portfolio)
        {
            TotalValue = portfolio.TotalValue;
            DayResult = portfolio.DayResult;
            DayResultPercentage = portfolio.DayResultPercentage;
            TotalProfitLoss = portfolio.TotalProfitLoss;
            TotalProfitLossPercentage = portfolio.TotalProfitLossPercentage;
            Currency = portfolio.Currency;
            LastUpdated = portfolio.LastUpdated;
            IsError = portfolio.IsError;
            ErrorMessage = portfolio.ErrorMessage;

            OnPropertyChanged(nameof(DayResultColor));
            OnPropertyChanged(nameof(TotalProfitLossColor));
        }

        public void RaisePropertyChangedForColors()
        {
            OnPropertyChanged(nameof(DayResult));
            OnPropertyChanged(nameof(DayResultPercentage));
            OnPropertyChanged(nameof(TotalProfitLoss));
            OnPropertyChanged(nameof(TotalProfitLossPercentage));
            OnPropertyChanged(nameof(TotalValue));
        }
    }
}
