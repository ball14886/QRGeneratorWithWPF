using System;
using System.Collections.Generic;
using System.IO;
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
using System.Drawing;

namespace PrintQRFillInPDF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        RunnerInfo _info = new RunnerInfo();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<RunnerInfo> infos = new List<RunnerInfo>();
            using (RunningQRDataContext r = new RunningQRDataContext())
            {
                infos = r.RunnerInfos.Where(x => x.RunnerID == 11731).ToList();
            }

            foreach (var info in infos)
            {
                int widthHeight = Convert.ToInt32(WidthHeightTextBox.Text.Trim());
                int fontSize = 12;
                int startX = (widthHeight / 2) - (widthHeight / 20);
                int startY = widthHeight - (fontSize * 2);

                _info = info;

                WebResponse response = default(WebResponse);
                Stream remoteStream = default(Stream);
                var text = $"http://runningqrapi.azurewebsites.net/RunningTimes/RecordByScanView?runnerIdentification={info.RunnerBIB}";
                var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chs={1}x{2}&chl={0}", text, WidthHeightTextBox.Text.Trim(), WidthHeightTextBox.Text.Trim());
                WebRequest request = WebRequest.Create(url);
                response = request.GetResponse();
                remoteStream = response.GetResponseStream();
                
                BitmapImage bitmapImage = new BitmapImage(response.ResponseUri);
                bitmapImage.DownloadCompleted += bitmapImage_DownloadCompleted;
                
                response.Close();
                remoteStream.Close();
            }
        }

        private void bitmapImage_DownloadCompleted(object sender, EventArgs e)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)sender));

            using (var filestream = new FileStream($"E:/Projects/MillionCreation/All QR/By version/QRCode4/{ _info.RunnerBIB }.jpg", FileMode.Create))
            {
                encoder.Save(filestream);
            }
        }
    }
}
