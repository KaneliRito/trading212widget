# Trading 212 Sticky Notes - Professional Desktop Applicatio

A modern, Proton-inspired Windows desktop application for real-time monitoring of your Trading 212 portfolio via elegant sticky notes.

![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple)
![WPF](https://img.shields.io/badge/WPF-Windows-blue)
![License](https://img.shields.io/badge/license-MIT-green)

## 📸 Screenshots

![Trading 212 Sticky Notes](https://via.placeholder.com/800x450/2B2A33/FFFFFF?text=Trading+212+Sticky+Notes)

## ✨ Features

- **Always-on-Top Sticky Notes**: Frameless, transparent windows that stay visible
- **Realtime Portfolio Tracking**: Automatic updates of your Trading 212 portfolio data
- **Proton Design System**: Sleek, minimalist design with dark/light theme support
- **Drag & Drop**: Freely positionable notes with automatic position saving
- **System Tray Integration**: Hide/show notes via system tray icon with extensive menu
- **Auto-start Support**: Optionally start automatically when Windows boots
- **Configurable**: Adjustable API endpoints and update intervals
- **Pin & Lock Controls**: Lock position or keep notes always on top
- **Error Handling**: Elegant error messages while retaining last known data

## 🚀 Getting Started

### Requirements

- Windows 10/11
- .NET 8.0 SDK or higher
- Trading 212 account (Invest or ISA)

### 📋 Creating a Trading 212 API Key

**Important**: You need an API key with the correct permissions to use this application.

1. **Open Trading 212 App or Website**
   - Log in to your Trading 212 account

2. **Navigate to API Settings**
   - Go to **Settings** → **API (Beta)**

3. **Generate a New API Key**
   - Click **"Generate new key"**
   - Give your key a recognizable name (e.g., "Sticky Notes App")

4. **Select the following permissions**:
   - ✅ **Account data** - Basic account information
   - ✅ **History** - Access to portfolio history
   - ✅ **History - dividends** - Dividend history
   - ✅ **History - orders** - Order history
   - ✅ **History - transactions** - Transaction history
   - ✅ **Metadata** - Account metadata
   - ✅ **Pies read** - Read pie portfolios
   - ✅ **Portfolio** - Portfolio data and positions

5. **Copy the API Key and Secret**
   - ⚠️ **Warning**: The API Key and Secret are only shown once!
   - Store them safely (e.g., in a password manager)

> **⚠️ Security**: Never share your API key and secret with others. They provide access to your portfolio data.

### 💾 Installation

1. **Download the latest release**
   ```bash
   # Clone the repository
   git clone https://github.com/yourusername/trading212stick.git
   cd trading212stick
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build --configuration Release
   ```

4. **Configure your API credentials**
   
   Edit `appsettings.json` in the project folder:
   ```json
   {
     "Trading212": {
       "ApiUrl": "https://live.trading212.com/api/v0/equity/account/summary",
       "ApiKey": "YOUR_API_KEY_HERE",
       "ApiSecret": "YOUR_API_SECRET_HERE",
       "UpdateIntervalSeconds": 60
     }
   }
   ```

5. **Start the application**
   ```bash
   dotnet run
   ```
   
   Of direct de executable:
   ```bash
   .\bin\Release\net8.0-windows\Trading212Stick.exe
   ```

## 🏗️ Architectuur

### Project Structuur

```
Trading212Stick/
├── Models/                      # Data modellen
│   ├── AppSettings.cs          # Configuratie structuur
│   ├── PortfolioInfo.cs        # Portfolio data model
│   └── Trading212ApiResponse.cs # API response mapping
│
├── ViewModels/                  # MVVM ViewModels
│   ├── ViewModelBase.cs        # Basis ViewModel met INotifyPropertyChanged
│   ├── RelayCommand.cs         # Command implementatie
│   ├── StickyNoteViewModel.cs  # ViewModel voor een enkele sticky note
│   └── MainViewModel.cs        # Hoofd ViewModel met timer logica
│
├── Views/                       # XAML Views
│   ├── StickyNoteWindow.xaml   # Sticky note UI
│   └── StickyNoteWindow.xaml.cs # Code-behind met drag & drop
│
├── Services/                    # Business Logic Layer
│   ├── ConfigurationService.cs # JSON configuratie beheer
│   ├── Trading212Service.cs    # HTTP client voor Trading 212 API
│   └── TrayIconService.cs      # System tray icon management
│
├── Converters/                  # XAML Value Converters
│   └── BoolToVisibilityConverter.cs
│
├── Themes/                      # Proton Design System
│   ├── DarkTheme.xaml          # Donker thema (default)
│   └── LightTheme.xaml         # Licht thema
│
├── Helpers/                     # Helper classes
│   └── IconHelper.cs           # Icon conversie voor tray
│
├── App.xaml                     # Application resources
├── App.xaml.cs                  # Application startup
├── appsettings.json            # Default configuratie
├── play_store_512.png          # App icon
└── Trading212Stick.csproj      # Project bestand
```

## 🎨 Gebruik

### Eerste Keer Opstarten

Bij de eerste keer opstarten:
1. De sticky note verschijnt rechtsboven op je scherm
2. Klik op het **⚙ Settings** icoon in de note om je API credentials in te voeren
3. Of bewerk direct `%AppData%\Trading212Stick\settings.json`

### System Tray Menu

Rechtermuisklik op het tray icon voor:

- **👁 Toon/Verberg Notes** - Beheer zichtbaarheid
- **📌 Note Controle**
  - Lock/Unlock (altijd op voorgrond)
  - Pin/Unpin (vergrendel positie)
- **🎨 Thema** - Wissel tussen Dark/Light mode
- **🚀 Start met Windows** - Auto-start bij opstarten
- **⚙ Configuratie**
  - API Instellingen bekijken
  - Configuratiemap openen
  - Configuratie herladen
- **❓ Help** - Over en documentatie

### Note Controls

Elke sticky note heeft:
- **⚙ Settings** - API configuratie aanpassen
- **🔒/🔓 Lock** - Houd note op voorgrond (of niet)
- **📌 Pin** - Vergrendel positie (voorkomt verslepen)
- **─ Minimize** - Minimaliseer naar taskbar
- **✕ Close** - Verberg note (blijft draaien in tray)

## 🔧 Configuration

### API Settings

After installation, you'll find the configuration in:
```
%AppData%\Trading212Stick\settings.json
```

Example configuration:
```json
{
  "Trading212": {
    "ApiUrl": "https://live.trading212.com/api/v0/equity/account/summary",
    "ApiKey": "your_api_key_here",
    "ApiSecret": "your_api_secret_here",
    "UpdateIntervalSeconds": 60
  },
  "WindowSettings": {
    "Theme": "Dark",
    "StartWithWindows": false
  },
  "SavedNotes": []
}
```

### Update Interval

- **Minimum recommended**: 30 seconds
- **Default**: 60 seconds
- **Warning**: Too frequent updates may lead to API rate limiting

## ⚠️ Troubleshooting

### "API Error: 401 Unauthorized"
- Check if your API key and secret are correctly configured
- Verify that the API key is still valid in Trading 212
- Make sure you're using the correct account type (Invest/ISA, not CFD)

### "API Error: 403 Forbidden"
- Check if your API key has **all required permissions**:
  - Account data
  - History (all types)
  - Metadata
  - Pies read
  - Portfolio
- Generate a new API key with the correct permissions if needed

### "Network error: No such host is known"
- Check your internet connection
- Verify the API URL in the configuration

### Notes don't appear
- Double-click the tray icon to show all notes
- Check if the saved position is outside the screen
- Delete `%AppData%\Trading212Stick\settings.json` to reset

## 🔒 Security

### API Key Protection

- ✅ API keys are stored locally (not in the cloud)
- ✅ Credentials are masked in the UI
- ⚠️ API keys are stored in plain text in `settings.json`
- 💡 **Tip**: Use read-only API keys where possible

### Best Practices

1. **Never share your API key** in code repositories
2. **Use .gitignore** for settings.json
3. **Backup your credentials** in a password manager
4. **Rotate your API key** periodically
5. **Regularly check** which apps have access

## 🏗️ Architectuur

### MVVM Pattern

De applicatie gebruikt strikte scheiding van concerns:

- **Models**: Pure data objecten zonder logica
- **ViewModels**: Business logica, data binding properties, commands
- **Views**: XAML UI zonder business logica

### Services Layer

- **ConfigurationService**: JSON configuratie beheer
## 🏗️ Architecture

### MVVM Pattern

The application uses strict separation of concerns:

- **Models**: Pure data objects without logic
- **ViewModels**: Business logic, data binding properties, commands
- **Views**: XAML UI without business logic

### Services Layer

- **ConfigurationService**: JSON configuration management
- **Trading212Service**: HTTP client for Trading 212 API
- **TrayIconService**: System tray icon management

### Async/Await Pattern

All HTTP calls use `async/await` to keep the UI responsive:

```csharp
private async Task UpdateAllNotesAsync()
{
    var portfolioData = await _tradingService.GetPortfolioDataAsync();
    // Update UI via data binding
}
```

## 🎨 Proton Design System

The application uses the **Proton design language** (inspired by Proton Mail/Drive):

### Colors

**Dark Theme (Default)**
- Primary: `#6D4AFF` (Proton Purple)
- Background: `#1C1B22` (Deep Dark)
- Surface: `#2B2A33` (Dark Gray)
- Text: `#FFFFFF` (White)
- Success: `#4CAF50` (Green)
- Error: `#F44336` (Red)

**Light Theme**
- Primary: `#6D4AFF` (Proton Purple)
- Background: `#FFFFFF` (White)
- Surface: `#F5F5F7` (Light Gray)
- Text: `#1C1B22` (Almost Black)

### Customization

Customize colors in `Themes/DarkTheme.xaml` or `Themes/LightTheme.xaml`:

```xml
<Color x:Key="ProtonPurple">#6D4AFF</Color>
```

Switch themes via the tray menu: 🎨 Theme → 🌙 Dark / ☀ Light
      "Id": "portfolio_main",
      "Left": 1550,
      "Top": 50,
      "Width": 320,
      "Height": 240,
      "IsVisible": true
    }
  ]
}
```

### Configuratie Locatie

Na eerste run wordt de configuratie opgeslagen in:
```
%AppData%\Trading212Stick\settings.json
```

### Update Interval Aanpassen

- Minimum aanbevolen: 30 seconden
- Default: 60 seconden
- Voor demo/testing: 10 seconden

**Let op**: Te frequente updates kunnen leiden tot API rate limiting!

## 📝 Extra Notes Toevoegen

Je kunt eenvoudig meerdere sticky notes toevoegen voor verschillende data:

### Stap 1: Nieuwe ViewModel Aanmaken

In `MainViewModel.cs` constructor:

```csharp
// Bestaande portfolio note
var portfolioNote = new StickyNoteViewModel("portfolio_main", "Trading 212 Portfolio");
Notes.Add(portfolioNote);

// NIEUW: Voeg extra note toe voor dagresultaat
var dayResultNote = new StickyNoteViewModel("day_result", "Dagresultaat");
Notes.Add(dayResultNote);

// NIEUW: Voeg note toe voor specifieke positie
var positionNote = new StickyNoteViewModel("position_aapl", "Apple (AAPL)");
Notes.Add(positionNote);
```

### Stap 2: Custom Data Tonen

Maak een custom service voor specifieke data of pas `StickyNoteViewModel` aan:

```csharp
// In StickyNoteViewModel.cs - voeg custom properties toe
public string SymbolName { get; set; }
public int Shares { get; set; }
public decimal CurrentPrice { get; set; }
```

### Stap 3: Custom XAML Template (Optioneel)

Maak een nieuwe View voor gespecialiseerde notes of gebruik DataTemplates.

## 🌐 System Tray Functies

Rechtermuisklik op het tray icon voor:

- **Toon Notes**: Alle notes zichtbaar maken
- **Verberg Notes**: Alle notes verbergen (blijven draaien op achtergrond)
- **Thema**: Schakel tussen Donker/Licht thema
- **Start met Windows**: Toggle auto-start functionaliteit
- **Instellingen**: Toon configuratie locatie en huidige settings
- **Afsluiten**: Sluit de applicatie volledig af

## 🛠️ Troubleshooting

### "API Error: 401 Unauthorized"
- Controleer of je API key correct is ingesteld
- Verifieer dat de API key nog geldig is in Trading 212

### "Netwerkfout: No such host is known"
- Controleer je internetverbinding
- Verifieer de API URL in de configuratie

### Notes verschijnen niet na opstarten
- Controleer of de opgeslagen positie niet buiten het scherm valt
- Verwijder `%AppData%\Trading212Stick\settings.json` om posities te resetten

### Update timer werkt niet
- Controleer of `UpdateIntervalSeconds` > 0 is in de configuratie
- Herstart de applicatie na configuratie wijzigingen

### Applicatie start niet
- Controleer of .NET 8.0 runtime is geïnstalleerd
- Run `dotnet --list-runtimes` om versies te verifiëren

## 🤝 Contributing

Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

### Development Setup

1. Fork the project
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

MIT License - see LICENSE file for details

## 🙏 Credits

- **Design Inspiration**: Proton Mail/Drive design system
- **Framework**: WPF (.NET 8.0)
- **API**: Trading 212 REST API
- **Icon**: Custom round icon with PNG conversion

## 📞 Support

For questions or issues:
1. Check the [Troubleshooting](#-troubleshooting) section
2. Review the [API documentation](https://t212public-api-docs.redoc.ly/)
3. Open a GitHub Issue
4. Check Trading 212 API status

## ⭐ Features Roadmap

- [ ] Multi-monitor support with per-monitor positioning
- [ ] Custom note templates via JSON configuration
- [ ] Chart widgets for portfolio performance
- [ ] Alert system for portfolio thresholds
- [ ] Export functionality to CSV/Excel
- [ ] Live price charts with graphs
- [ ] Notification center for important events
- [ ] Widget for individual positions
- [ ] Dark/Light theme auto-switch based on Windows theme

---

**⚠️ Disclaimer**: This application is not officially connected with Trading 212. Use at your own risk. The developers are not responsible for any loss or damage from using this software.

**Made with ❤️ and C#**

© 2026 - Trading 212 Sticky Notes

### Portfolio Overview (Gebruikt)
```
GET https://live.trading212.com/api/v0/equity/portfolio
```

Response:
```json
{
  "cash": 1234.56,
  "invested": 10000.00,
  "ppl": 567.89,
  "result": 123.45,
  "resultCoef": 0.0123,
  "total": 11234.45
}
```

### Accounts (Voor toekomstige uitbreiding)
```
GET https://live.trading212.com/api/v0/equity/account/info
```

### Positions (Voor specifieke aandelen notes)
```
GET https://live.trading212.com/api/v0/equity/portfolio/positions
```

## 🔐 Security Best Practices

1. **Bewaar je API key veilig**: Deel deze nooit in code repositories
2. **Gebruik readonly API keys**: Indien mogelijk, gebruik keys met alleen-lezen permissies
3. **Encryptie**: Overweeg API key encryptie voor productie gebruik
4. **Rate Limiting**: Houd je aan de API rate limits van Trading 212

## 🚀 Geplande Features

- [ ] Multi-monitor support met per-monitor positionering
- [ ] Custom note templates via JSON configuratie
- [ ] Grafiek widgets voor portfolio performance
- [ ] Alert systeem bij portfolio drempels
- [ ] Export functionaliteit naar CSV/Excel
- [ ] Meer API providers (DeGiro, Interactive Brokers, etc.)
- [ ] Live price charts met grafieken
- [ ] Notification center voor belangrijke events

## 🤝 Contributing

Pull requests zijn welkom! Voor grote wijzigingen, open eerst een issue om te bespreken wat je wilt wijzigen.

## 📄 Licentie

MIT License - zie LICENSE bestand voor details

## 🙏 Credits

- **Design Inspiratie**: Proton Mail/Drive design system
- **Framework**: WPF (.NET 8.0)
- **API**: Trading 212 REST API
- **Icons**: Windows System Icons (vervang met custom icons voor productie)

## 📞 Support

Voor vragen of problemen:
1. Check de Troubleshooting sectie hierboven
2. Open een GitHub Issue
3. Controleer de Trading 212 API documentatie

---

**⚠️ Disclaimer**: Deze applicatie is niet officieel verbonden met Trading 212. Gebruik op eigen risico. De ontwikkelaars zijn niet verantwoordelijk voor verlies of schade door het gebruik van deze software.

**Made with ❤️ and C#**
