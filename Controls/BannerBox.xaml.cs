using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using WpfAnimatedGif;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Interop;
using System.Net;

namespace _8Banner.Controls
{
	/// <summary>
	/// Interaction logic for BannerBox.xaml
	/// </summary>
	public partial class BannerBox : UserControl
	{
		BitmapImage bannerImage;
		string bannerUrl;
		string filepath;

		public BannerBox(BitmapImage bannerImage, string bannerUrl)
		{
			InitializeComponent();

			this.bannerImage = bannerImage;
			this.bannerUrl = bannerUrl;
		}

		private void banner_Loaded(object sender, RoutedEventArgs e)
		{
			if (bannerUrl.EndsWith(".gif"))
				ImageBehavior.SetAnimatedSource(this.banner, bannerImage);
			else
				this.banner.Source = bannerImage;
		}

		private void openImage_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(bannerUrl);
		}

		private void saveAs_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();

			saveFileDialog.FileName = bannerUrl.Replace("https://banners.8ch.net/banners/" + HomePage.BoardName + "/", "");
			saveFileDialog.Filter = "Image File|*.png;*.jpg;*.jpeg;*.gif";

			if (saveFileDialog.ShowDialog() == true)
			{
				filepath = saveFileDialog.FileName;
				
				FileStream stream = new FileStream(filepath, FileMode.Create);
				
				if (filepath.ToLower().EndsWith(".gif"))
				{
					SaveImage(new GifBitmapEncoder(), stream);
				}
				else if (filepath.ToLower().EndsWith(".png"))
				{
					SaveImage(new PngBitmapEncoder(), stream);
				}
				else if (filepath.ToLower().EndsWith(".jpg") || filepath.ToLower().EndsWith(".jpeg"))
				{
					SaveImage(new JpegBitmapEncoder(), stream);
				}
				stream.Close();
			}
		}

		private void copyImageUrl_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(bannerUrl);
		}

		private void copyImage_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetImage(bannerImage);
		}

		private void SaveImage(BitmapEncoder encoder, FileStream fileStream)
		{
			if (encoder.GetType() == typeof(GifBitmapEncoder))
			{
				try
				{
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(bannerUrl);
					
					using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
					using (Stream responseStream = response.GetResponseStream())
					{ 
						responseStream.CopyTo(fileStream);
					}
				}
				catch
				{
					MainWindow.main.Status = "Saving Failed - Connection Lost";
				}
			}
			else
			{
				encoder.Frames.Add(BitmapFrame.Create(bannerImage));
				encoder.Save(fileStream);
			}
		}
	}
}
