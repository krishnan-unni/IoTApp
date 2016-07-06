using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
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
        public CameraViewer()
        {
            this.InitializeComponent();
            CameraUI = new CameraCaptureUI();
            CameraUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            CameraUI.PhotoSettings.CroppedSizeInPixels = new Size(400, 400);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void picCapture_Click(object sender, RoutedEventArgs e)
        {
            StorageFile photo = await CameraUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (photo==null)
            {
                return;
            }
            IRandomAccessStream photoStream = await photo.OpenAsync(FileAccessMode.Read);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(photoStream);
            SoftwareBitmap bitmap = await decoder.GetSoftwareBitmapAsync();

            SoftwareBitmap bitmapBGR8 = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

            SoftwareBitmapSource imageSource = new SoftwareBitmapSource();
            await imageSource.SetBitmapAsync(bitmapBGR8);

            cameraView.Source = imageSource;
        }
    }
}
