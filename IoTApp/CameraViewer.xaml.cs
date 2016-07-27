using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Azure.Devices.Client;
using Microsoft.IoT.Lightning;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CameraViewer : Page
    {
        static DeviceClient deviceClient = null;

        public MediaCapture mediaCapture;
        private MediaCaptureInitializationSettings settings;
        bool isInitialized = false;

        byte[] imgdata;

        static string AzureHubHostName = "INet.azure-devices.net";
        static string deviceKey = "X+K88nC/NzxKu4pfesVfc3DlOBclviGlck4G50wgxLU=";
        static string deviceId = "TestIoTDevice";

        public CameraViewer()
        {
            this.InitializeComponent();
            try
            {
                deviceClient = DeviceClient.Create(AzureHubHostName, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));

                InitializeMediaCapture();
            }
            catch (Exception e)
            { }
        }

        private async void InitializeMediaCapture()
        {

            //find all cameras
            var cameras = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            if (cameras != null)
            {
                //get first camera
                DeviceInformation camera = cameras.FirstOrDefault();

                //initiate settings
                settings = new MediaCaptureInitializationSettings() { VideoDeviceId = camera.Id };
                mediaCapture = new MediaCapture();
                try
                {
                    //initialize media capture object
                    await mediaCapture.InitializeAsync(settings);
                    isInitialized = true;

                    var resolution = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo);
                    await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, resolution.ToList()[1]);
                    preview.Source = mediaCapture;
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("The app was denied access to the camera");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception when initializing MediaCapture with {0}: {1}", camera.Id, ex.ToString());
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void medPicCapture_Click(object sender, RoutedEventArgs e)
        {
            if (isInitialized)
            {
                InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
                await mediaCapture.StartPreviewAsync();
                await mediaCapture.CapturePhotoToStreamAsync(Windows.Media.MediaProperties.ImageEncodingProperties.CreateBmp(), stream);
                await mediaCapture.StopPreviewAsync();
                if (stream != null)
                {
                    SoftwareBitmap bitmap = await GetBitmap(stream);
                    await SetBitMaptoImage(bitmap);
                    imgdata = ConvertToBytes(bitmap);
                }
                MainPage.SendMessageToCloudAsync(imgdata);
            }
        }

        private void copyPicCapture_Click(object sender, RoutedEventArgs e)
        {
            copycameraView.Source = ConvertToSoftwareBitmap(imgdata);
        }

        /// <summary>
        /// Get the bitmap out of the RandomAccessStream
        /// </summary>
        /// <param name="photoStream"></param>
        /// <returns></returns>
        private static async Task<SoftwareBitmap> GetBitmap(IRandomAccessStream photoStream)
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(photoStream);
            SoftwareBitmap bitmap = await decoder.GetSoftwareBitmapAsync();
            return bitmap;
        }

        private async Task SetBitMaptoImage(SoftwareBitmap bitmap)
        {
            SoftwareBitmap bitmapBGR8 = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

            SoftwareBitmapSource imageSource = new SoftwareBitmapSource();
            await imageSource.SetBitmapAsync(bitmapBGR8);

            cameraView.Source = imageSource;

        }

        public byte[] ConvertToBytes(SoftwareBitmap bitmap)
        {
            byte[] data;
            WriteableBitmap newBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
            bitmap.CopyToBuffer(newBitmap.PixelBuffer);

            using (Stream stream = newBitmap.PixelBuffer.AsStream())
            using (MemoryStream memStream = new MemoryStream())
            {
                stream.CopyTo(memStream);
                data = memStream.ToArray();
            }
            return data;
        }

        public WriteableBitmap ConvertToSoftwareBitmap(byte[] data)
        {
            WriteableBitmap bitmapNew = new WriteableBitmap(640, 480);
            using (Stream stream = bitmapNew.PixelBuffer.AsStream())
            using (MemoryStream memStream = new MemoryStream(data))
            {
                memStream.CopyTo(stream);
            }
            return bitmapNew;
        }
    }
}
