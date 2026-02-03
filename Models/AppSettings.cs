using System.Collections.Generic;

namespace Trading212Stick.Models
{
    public class AppSettings
    {
        public Trading212Settings Trading212 { get; set; } = new();
        public WindowSettings WindowSettings { get; set; } = new();
        public List<SavedNoteData> SavedNotes { get; set; } = new();
    }

    public class Trading212Settings
    {
        public string ApiUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public int UpdateIntervalSeconds { get; set; } = 60;
    }

    public class WindowSettings
    {
        public string Theme { get; set; } = "Dark";
        public bool StartWithWindows { get; set; }
    }

    public class SavedNoteData
    {
        public string Id { get; set; } = string.Empty;
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}
