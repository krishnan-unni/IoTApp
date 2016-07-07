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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CameraViewer : Page
    {
        public CameraCaptureUI CameraUI;
        public MediaCapture mediaCapture;
        private MediaCaptureInitializationSettings settings;
        bool isInitialized = false;

        byte[] imgdata;
        public CameraViewer()
        {
            this.InitializeComponent();
            CameraUI = new CameraCaptureUI();
            CameraUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            CameraUI.PhotoSettings.CroppedSizeInPixels = new Size(400, 400);

            InitializeMediaCapture();
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
        /// Using MediaCapture to get pics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void medCapture_Click(object sender, RoutedEventArgs e)
        {
            if (isInitialized)
            {

                await mediaCapture.StartPreviewAsync();
                VideoFrame frame = await mediaCapture.GetPreviewFrameAsync();
                //if (frame != null)
                //{
                //    await SetBitMaptoImage(frame.SoftwareBitmap);
                //}
                await mediaCapture.StopPreviewAsync();
            }
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
                await mediaCapture.CapturePhotoToStreamAsync(Windows.Media.MediaProperties.ImageEncodingProperties.CreateJpeg(), stream);
                await mediaCapture.StopPreviewAsync();
                if (stream != null)
                {
                    var str = stream;
                    SoftwareBitmap bitmap = await GetBitmap(stream);
                    await SetBitMaptoImage(bitmap);
                    imgdata = ConvertToBytes(bitmap);
                }
            }
        }

        /// <summary>
        /// Using CaptureCameraUI to get pics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void picCapture_Click(object sender, RoutedEventArgs e)
        {
            StorageFile photo = await CameraUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (photo == null)
            {
                return;
            }
            IRandomAccessStream photoStream = await photo.OpenAsync(FileAccessMode.Read);
            SoftwareBitmap bitmap = await GetBitmap(photoStream);

            await SetBitMaptoImage(bitmap);
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
            //SoftwareBitmap bitmap = SoftwareBitmap.CreateCopyFromBuffer()
            //BitmapImage image = new BitmapImage();
            WriteableBitmap bitmapNew = new WriteableBitmap(640, 480);
            //using (InMemoryRandomAccessStream newMem = new InMemoryRandomAccessStream())
            using (Stream stream = bitmapNew.PixelBuffer.AsStream())
            using (MemoryStream memStream = new MemoryStream(data))
            {
                memStream.CopyTo(stream);
                
            }
            return bitmapNew;
        }

        private void copyPicCapture_Click(object sender, RoutedEventArgs e)
        {

            copycameraView.Source = ConvertToSoftwareBitmap(imgdata);
        }
    }
}
