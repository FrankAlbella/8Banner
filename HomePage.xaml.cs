using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _8Banner
{
	public partial class HomePage : Page
	{
		internal static HomePage page;

		string boardAddress;
		public static string BoardName;

		GetImageWorker giw;

		public HomePage()
		{
			InitializeComponent();
			page = this;
		}

		private void boardBox_Enter(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SetupWindow();
			}
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			SetupWindow();
		}

		private void SetupWindow()
		{
			boardAddress = @"https://banners.8ch.net/random/" + boardBox.Text;

			if (BoardExists(boardBox.Text))
			{
				MainWindow.main.Status = "Preparing...";

				MainWindow.main.frame.Navigate(new BannerPage());

				MainWindow.main.btnCancel.Visibility = Visibility.Visible;

				giw = new GetImageWorker();
				giw.Start();
			}
			else
			{
				MessageBox.Show("Please enter a valid board");
			}
		}

		private bool BoardExists(string newBoardName)
		{
			MainWindow.main.Status = "Checking if board exists...";

			newBoardName.Replace("/", "");

			BoardName = newBoardName.ToLower();

			try
			{
				if (newBoardName == "")
					throw new Exception();
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://8ch.net/"
							+ BoardName
							+ @"/index.html");

				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				response.Close();
			}
			catch
			{
				return false;
			}
			finally
			{
				MainWindow.main.Status = "Done!";
			}
			return true;

		}

		internal void CancelTask()
		{
			giw.Cancel();
		}
	}
}
