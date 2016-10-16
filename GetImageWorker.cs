using _8Banner.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace _8Banner
{
    class GetImageWorker
    {
        BackgroundWorker worker;

        List<string> bannerUrls = new List<string>();

		string defaultBannerUrl = "https://banners.8ch.net/default.png";

		string boardName;

		string bannerUrl;

		int bannerLimit = 150;

		bool working;

        public GetImageWorker()
        {
            worker = new BackgroundWorker();

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            boardName = HomePage.BoardName;
        }
		
		public void Start()
        {
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
			int bannersAdded = 0;
			working = true;
			while (bannersAdded < bannerLimit)
			{
				if (worker.CancellationPending)
				{
					e.Cancel = true;
					break;
				}
				try
				{
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://banners.8ch.net/random/" + boardName);

					using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
					{
						bannerUrl = response.ResponseUri.ToString();
					}

					if (bannerUrl == defaultBannerUrl)
						worker.CancelAsync();
					else if (!bannerUrls.Contains(bannerUrl))
					{
						try
						{
							Application.Current.Dispatcher.Invoke((Action)delegate
							{

								MainWindow.main.Status = "Adding banners...";

								BitmapImage bitmapBanner = new BitmapImage();
								bitmapBanner.BeginInit();
								bitmapBanner.UriSource = new Uri(bannerUrl, UriKind.Absolute);
								bitmapBanner.EndInit();

								BannerPage.page.bannerPanel.Children.Add(new BannerBox(bitmapBanner, bannerUrl));

								bannerUrls.Add(bannerUrl);

								bannersAdded++;
							});
						}
						catch (Exception exception)
						{
							MainWindow.main.Status = String.Format("{0} threw an expection", exception.Source);
							MessageBox.Show(String.Format("{0} causing exception: {1} ", exception.Source, exception.ToString()));
						}
					}
				}
				catch
				{
					MainWindow.main.Status = "Connection Lost - Retrying...";
				}
			}
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
			Clear();
			if (e.Cancelled)
			{
				Application.Current.Dispatcher.Invoke((Action)delegate
				{
					MainWindow.main.btnCancel.Content = "Reset";
				});
				MainWindow.main.Status = "Canceled";
			}
			else
			{
				MainWindow.main.Status = "Complete";
				Application.Current.Dispatcher.Invoke((Action)delegate
				{
					MainWindow.main.btnCancel.Content = "Reset";
				});
			}
        }

		public void Cancel()
		{
			worker.CancelAsync();
		}

		public bool isWorking()
		{
			return working;
		}

		public void Clear()
		{
			bannerUrls.Clear();
			working = false;
		}
    }
}
