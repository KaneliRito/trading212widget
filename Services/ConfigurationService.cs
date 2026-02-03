using System;
using System.IO;
using Newtonsoft.Json;
using Trading212Stick.Models;

namespace Trading212Stick.Services
{
    public class ConfigurationService
    {
        private readonly string _configPath;
        private AppSettings _settings;

        public ConfigurationService()
        {
            var appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Trading212Stick"
            );

            Directory.CreateDirectory(appDataFolder);
            _configPath = Path.Combine(appDataFolder, "settings.json");

            LoadSettings();
        }

        public AppSettings Settings => _settings;

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var json = File.ReadAllText(_configPath);
                    _settings = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
                }
                else
                {
                    // Laad defaults van appsettings.json in project folder
                    var defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                    if (File.Exists(defaultPath))
                    {
                        var json = File.ReadAllText(defaultPath);
                        _settings = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
                    }
                    else
                    {
                        _settings = new AppSettings();
                    }
                    
                    SaveSettings();
                }
            }
            catch
            {
                _settings = new AppSettings();
            }
        }

        public void SaveSettings()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
                File.WriteAllText(_configPath, json);
            }
            catch
            {
                // Log error indien nodig
            }
        }

        public void UpdateNotePosition(string noteId, double left, double top, double width, double height)
        {
            var existingNote = _settings.SavedNotes.Find(n => n.Id == noteId);
            if (existingNote != null)
            {
                existingNote.Left = left;
                existingNote.Top = top;
                existingNote.Width = width;
                existingNote.Height = height;
            }
            else
            {
                _settings.SavedNotes.Add(new SavedNoteData
                {
                    Id = noteId,
                    Left = left,
                    Top = top,
                    Width = width,
                    Height = height
                });
            }

            SaveSettings();
        }
    }
}
