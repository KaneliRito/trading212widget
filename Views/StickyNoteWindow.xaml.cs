using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Trading212Stick.Services;
using Trading212Stick.ViewModels;

namespace Trading212Stick.Views
{
    public partial class StickyNoteWindow : Window
    {
        private readonly ConfigurationService _configService;
        private bool _isDragging;

        public StickyNoteWindow(StickyNoteViewModel viewModel, ConfigurationService configService)
        {
            InitializeComponent();
            DataContext = viewModel;
            _configService = configService;

            // Load saved position
            LoadWindowPosition(viewModel.NoteId);

            // Events
            Loaded += StickyNoteWindow_Loaded;
            Closing += StickyNoteWindow_Closing;
        }

        private void StickyNoteWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Ensure window is visible on screen
            EnsureWindowOnScreen();
        }

        private void StickyNoteWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveWindowPosition();
        }

        private void LoadWindowPosition(string noteId)
        {
            var savedNote = _configService.Settings.SavedNotes.Find(n => n.Id == noteId);
            if (savedNote != null)
            {
                Left = savedNote.Left;
                Top = savedNote.Top;
                Width = savedNote.Width > 0 ? savedNote.Width : 320;
                Height = savedNote.Height > 0 ? savedNote.Height : 240;
            }
            else
            {
                // Default position (center right of screen)
                Width = 320;
                Height = 240;
                Left = SystemParameters.PrimaryScreenWidth - Width - 50;
                Top = 50;
            }
        }

        private void SaveWindowPosition()
        {
            if (DataContext is StickyNoteViewModel vm)
            {
                _configService.UpdateNotePosition(vm.NoteId, Left, Top, Width, Height);
            }
        }

        private void EnsureWindowOnScreen()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            if (Left + Width > screenWidth)
                Left = screenWidth - Width - 10;
            if (Top + Height > screenHeight)
                Top = screenHeight - Height - 10;
            if (Left < 0)
                Left = 10;
            if (Top < 0)
                Top = 10;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is StickyNoteViewModel vm && vm.IsPinned)
                return; // Niet verslepen als gepind
            if (e.ClickCount == 1)
            {
                _isDragging = true;
                DragMove();
            }
        }

        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                SaveWindowPosition();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
