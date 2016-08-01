using IoTApp.Sensors;
using System;
using System.Linq;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.IoT.Lightning.Providers;
using Windows.Devices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RGBLed : Page
    {
        const int R_PIN = 4;
        const int G_PIN = 17;
        const int B_PIN = 18;

        RGBLedController led;
        DispatcherTimer timer;
        int counter = 0;
        Color[] colors = new Color[] { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Orange, Colors.Turquoise, Colors.White, Colors.Pink };

        public RGBLed()
        {
            this.InitializeComponent();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
        }

        private void Timer_Tick(object sender, object e)
        {
            var pos = counter++ % colors.Length;
            led.Color = colors[pos];
        }

        private async void RGBLedPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Devices.DevicesLowLevelContract", 1))
            {
                try
                {
                    var gpio = GpioController.GetDefaultAsync();
                    if (gpio != null)
                    {
                        if (LightningProvider.IsLightningEnabled)
                        {
                            LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
                        }
                        //need to add pwm software to add pwm support
                        var pwmControllers = await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider());

                        PwmController pwmController;

                        if (pwmControllers != null)
                        {
                            pwmController = pwmControllers.FirstOrDefault();

                            if (pwmController != null)
                            {
                                pwmController.SetDesiredFrequency(100);
                                PwmPin Rpin = pwmController.OpenPin(R_PIN);
                                PwmPin Gpin = pwmController.OpenPin(G_PIN);
                                PwmPin Bpin = pwmController.OpenPin(B_PIN);
                                led = new RGBLedController(Rpin, Gpin, Bpin);
                                led.On();
                                led.Color = Colors.Coral;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("No pwm controller");
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No GPIO controller");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                timer.Start();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No IoT support");
            }
        }

        private void RGBLedPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (led != null)
            {
                led.Off();
                led.Dispose();
            }
        }
    }
}
