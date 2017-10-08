using System;
using System.Diagnostics;
using QREET.Lib;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using ZXing;
using ZXing.QrCode;

namespace QREET.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ConverToCodeClick(object sender, RoutedEventArgs e)
        {
            try
            {
                EetData eet = new EetData();
                eet.BKP = BkpBox.Text;
                eet.DIC = ulong.Parse(DicBox.Text);
                eet.Price = decimal.Parse(PriceBox.Text);
                eet.Date = DateBox.Date.Date.Add(TimeBox.Time);

                eet.Validate();

                EetBinData bin = eet.ToBinData();
                byte[] bytes = bin.GetBytes();
                string hex = BitConverter.ToString(bytes);
                CodeBox.Text = hex.Replace("-", "");

                CreateQrCode(bytes);

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                throw;
            }
        }

        private void CreateQrCode(byte[] array)
        {
            BarcodeWriter write = new BarcodeWriter();
            write.Options = new QrCodeEncodingOptions { Height = 100, Width = 100 };
            write.Format = BarcodeFormat.QR_CODE;
            WriteableBitmap wb = write.Write(Convert.ToBase64String(array));
            QR.Source = wb;
        }

        private void ConvertBackClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}