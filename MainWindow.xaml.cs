using _8Banner.Controls;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _8Banner
{
    public partial class MainWindow : Window
    {
        internal static MainWindow main;

		

		internal string Status
        {
            get { return lblStatus.Content.ToString(); }
            set { Dispatcher.Invoke(new Action(() => { lblStatus.Content = value; })); }
        }

        public MainWindow()
        {
            InitializeComponent();
            main = this;
		}

		internal void ResetWindow()
		{
			frame.Navigate(new HomePage());

			Status = "Ready";
			btnCancel.Visibility = Visibility.Hidden;
			btnCancel.Content = "Cancel";
		}

        private void lblStatus_Loaded(object sender, RoutedEventArgs e)
        {
            Label label = (Label)sender;

			label.Content = String.Format("Height: {0}, Width: {1}", label.ActualHeight, label.ActualWidth);
            label.Background = Brushes.LightGray;
        }

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			if (btnCancel.Content.ToString() == "Cancel")
				HomePage.page.CancelTask();
			else
				ResetWindow();
		}

		private void frame_Loaded(object sender, RoutedEventArgs e)
		{
			frame.Navigate(new HomePage());
		}
	}
}
