using System.Windows;
using System;
using System.Windows.Forms;

namespace SwtorUnleashed.View
{
    /// <summary>
    /// Interaction logic for LauncherWindow.xaml
    /// </summary>
    public partial class LauncherWindow : Window
    {
		public LauncherWindow()
        {
            InitializeComponent();
            Left      = Math.Max((SystemInformation.PrimaryMonitorSize.Width - 600) / 2, 0);
			Top       = Math.Max((SystemInformation.PrimaryMonitorSize.Height - 750) / 2, 0);
            Activated += launcher_Activated;
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	DragMove();
        }

        private void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            ShowWindow<AboutWindow>();
        }

        private void buttonSettings_Click(object sender, RoutedEventArgs e)
        {
           ShowWindow<SetupWindow>();
        }

        private Window _win;

        private void ShowWindow<TWindow>()
            where TWindow : Window, new()
        {
            _win = new TWindow
                {
                    Top           = Top + Height,
                    Left          = Left,
                    Owner         = this,
                    ShowInTaskbar = false
                };
            _win.LocationChanged += win_LocationChanged;
            _win.Activated       += win_Activated;
            _win.ShowDialog();
            _win = null;
        }

        void launcher_Activated(object sender, EventArgs e)
        {
            if (_win != null)
                _win.Activate();
        }

        void win_Activated(object sender, EventArgs e)
        {
            if (_win != null)
                Activate();
        }

        void win_LocationChanged(object sender, EventArgs e)
        {
            Top  = _win.Top - Height;
            Left = _win.Left;
        }

        private void buttonQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
