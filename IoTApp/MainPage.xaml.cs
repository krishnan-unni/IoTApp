﻿using System;
using Windows.UI.Xaml.Controls;
using Microsoft.Azure.Devices.Client;
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

        static MainPage()
        {
            deviceClient = DeviceClient.Create(AzureHubHostName, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));
        }

        public MainPage()
        {
            this.InitializeComponent();
            try
            {
                ReceiveCommandFromCloudAsync();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public static async void SendMessageToCloudAsync(byte[] data)
        {
            //Message message = new Message(Encoding.UTF8.GetBytes("This is just as message"));
            //message.Properties["messageType"] = "string";
            //await deviceClient.SendEventAsync(message);
            try
            {
                Message messageCmd = new Message(data);
                messageCmd.MessageId = Guid.NewGuid().ToString();
                messageCmd.Properties["messageType"] = "byte";
                //messageCmd.Properties["Trigger"] = "ImageCapture";
                //messageCmd.Properties["Body"] = "Here is the picture of fridge";
                //messageCmd.Properties["Command"] = "GetImage";

                await deviceClient.SendEventAsync(messageCmd);
            }
            catch (Exception e)
            {
                
            }

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
