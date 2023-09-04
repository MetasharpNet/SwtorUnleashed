using System.Windows;

namespace SwtorUnleashed.View
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window
	{
		public AboutWindow()
		{
			InitializeComponent();
			// Insert code required on object creation below this point.
		}

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	DragMove();
        }

		private void buttonClose_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}