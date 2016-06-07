using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;
using System.Text;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static DeviceClient deviceClient = null;
        static string AzureHubHostName = "INet.azure-devices.net";
        static string deviceKey = "X+K88nC/NzxKu4pfesVfc3DlOBclviGlck4G50wgxLU=";
        static string deviceId = "TestIoTDevice";
        static string AzureHubConnectionString = "HostName=INet.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=TrUhTc0eM+ls5NkUT7W5r74xpA98FiMnqNjM93kX7Z0=";

        public MainPage()
        {
            this.InitializeComponent();
            try
            {
                deviceClient = DeviceClient.Create(AzureHubHostName, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));

                SendMessageToCloudAsync();

            }
            catch (Exception e)
            {
                throw;
            }

        }

        private static async void SendMessageToCloudAsync()
        {
            Message messageCmd = new Message();
            messageCmd.Properties["Trigger"] = "ImageCaptured";
            messageCmd.Properties["Body"] = "Here is the picture of fridge";
            messageCmd.Properties["Command"] = "GetImage";

            await deviceClient.SendEventAsync(messageCmd);

            Message message = new Message(Encoding.UTF8.GetBytes("This is just as message"));

            await deviceClient.SendEventAsync(message);
        }
    }
}
