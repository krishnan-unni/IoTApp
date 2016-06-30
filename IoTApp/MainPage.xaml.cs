using System;
using Windows.UI.Xaml.Controls;
using Microsoft.Azure.Devices.Client;
using System.Text;
using IoTApp.Sensors;

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

        public MainPage()
        {
            this.InitializeComponent();
            try
            {
                deviceClient = DeviceClient.Create(AzureHubHostName, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));

                ReceiveCommandFromCloudAsync();

            }
            catch (Exception e)
            {
                throw;
            }

        }

        private static async void SendMessageToCloudAsync()
        {
            Message messageCmd = new Message();
            messageCmd.MessageId = Guid.NewGuid().ToString();
            messageCmd.Properties["messageType"] = "interactive";
            messageCmd.Properties["Trigger"] = "ImageCapture";
            messageCmd.Properties["Body"] = "Here is the picture of fridge";
            messageCmd.Properties["Command"] = "GetImage";

            await deviceClient.SendEventAsync(messageCmd);

            Message message = new Message(Encoding.UTF8.GetBytes("This is just as message"));

            await deviceClient.SendEventAsync(message);
        }

        private static async void ReceiveCommandFromCloudAsync()
        {
            bool IsPiInitiated = false;
            
            while (true)
            {
                Message command = await deviceClient.ReceiveAsync();

                if (command == null) continue;

                if (command.Properties["messageType"] == "interactive")
                {
                    //process command
                    if (!IsPiInitiated)
                    {
                        IsPiInitiated = PiController.InitializeGPIO();
                    }
                    if (IsPiInitiated && command.Properties["Command"] == "turnOff")
                    {
                        PiController.TurnOffLED();
                    }
                    else if (IsPiInitiated && command.Properties["Command"] == "turnOn")
                    {
                        PiController.TurnOnLED();
                    }

                    await deviceClient.CompleteAsync(command);
                    continue;
                }

                //do some other operation and complete
                await deviceClient.CompleteAsync(command);
                continue;
            }
        }
    }
}
