using System;
using System.Diagnostics;
using System.Globalization;
using Windows.Devices.Enumeration;
using QREET.Lib;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ZXing;
using ZXing.Mobile;
using ZXing.QrCode;
using BarcodeWriter = ZXing.Mobile.BarcodeWriter;

namespace QREET.UWP
{
    public sealed partial class MainPage
    {
        private readonly MobileBarcodeScanner scanner;
        private bool cameraAvailable;

        public MainPage()
        {
            InitializeComponent();

            //Create a new instance of our scanner
            scanner = new MobileBarcodeScanner(this.Dispatcher);
            scanner.Dispatcher = this.Dispatcher;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Input.FikBox.Text = "a7764953-8ae3-ba50";
            Input.BkpBox.Text = "8ae3ba50-a7764953";
            Input.DicBox.Text = "00685976";
            Input.PriceBox.Text = "650.35";
            Input.DateBox.Date = new DateTimeOffset(new DateTime(2017, 10, 5));
            Input.TimeBox.Time = new TimeSpan(13, 27, 00);
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            cameraAvailable = devices.Count > 0;
        }

        private async void ConverToCodeClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string fik = Input.FikBox.Text.Replace("-", "");
                string bkp = Input.BkpBox.Text.Replace("-", "");
                string dic = Input.DicBox.Text;
                string kc = Input.PriceBox.Text;
                string dt = Input.DateBox.Date.Date.Add(Input.TimeBox.Time).ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture);
                string mode = Input.SimplifiedRadio.IsChecked == true ? "Z" : "B";

                if (EetReceipt.TryCreate(fik, bkp, dic, kc, dt, mode, out EetReceipt eet, out string msg))
                {
                    string qreet = eet.ToQREETString();
                    CodeBox.Text = qreet;
                    CreateQrCode(qreet);
                }
                else
                {
                    MessageDialog md = new MessageDialog(msg, "Error");
                    await md.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.ToString(), "Error");
                await md.ShowAsync();
                Debug.WriteLine(ex);
            }
        }

        private async void ConvertBackClick(object sender, RoutedEventArgs e)
        {
            if (!cameraAvailable)
            {
                MessageDialog md = new MessageDialog("Nenalezena žádná kamera, kterou by šlo číst´QR kódy.", "Chyba");
                await md.ShowAsync();
                return;
            }
            scanner.UseCustomOverlay = false;
            scanner.TopText = "Podržte kameru před QR kódem.";
            scanner.BottomText = "Kamera načte kód automaticky. Pro návrat stiskněte tlačítko Zpět.";
            scanner.ScanContinuously(OnScan);
        }

        private async void OnScan(Result result)
        {
            try
            {
                if (result?.Text == null) return;
                string qreet = result.Text;

                if (EetReceipt.TryCreate(qreet, out EetReceipt eet, out string msg))
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        CodeBox2.Text = qreet;
                        Output.FikBox.Text = eet.FIK ?? "";
                        Output.BkpBox.Text = eet.BKP ?? "";
                        Output.DicBox.Text = eet.DIC ?? "";
                        Output.PriceBox.Text = eet.Price.ToString("0.##", CultureInfo.InvariantCulture);
                        Output.DateBox.Date = eet.Date.Date;
                        Output.TimeBox.Time = eet.Date.TimeOfDay;
                        Output.SimplifiedRadio.IsChecked = eet.Mode == EetSaleMode.Simplified;
                        scanner.Cancel();
                    });
                }
                else
                {
                    MessageDialog md = new MessageDialog(msg, "Error");
                    await md.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.ToString(), "Error");
                await md.ShowAsync();
                Debug.WriteLine(ex);
            }
        }

        private void CreateQrCode(string code)
        {
            BarcodeWriter write = new BarcodeWriter();
            write.Options = new QrCodeEncodingOptions { Height = 200, Width = 200 };
            write.Format = BarcodeFormat.QR_CODE;
            WriteableBitmap wb = write.Write(code);
            QR.Source = wb;
        }
    }
}