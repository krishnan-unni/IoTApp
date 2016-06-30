using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Gpio;
using IoTApp.Sensors;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LEDSample : Page
    {
        DispatcherTimer ledTimer;
        bool GPIOset = false;

        public LEDSample()
        {
            this.InitializeComponent();

            ledTimer = new DispatcherTimer();
            ledTimer.Interval = new System.TimeSpan(0, 0, 0, 1);
            ledTimer.Tick += LedTimer_Tick;

            if (PiController.InitializeGPIO())
            {
                ledTimer.Start();
            }
        }

        private void LedTimer_Tick(object sender, object e)
        {
            if (PiController.pinValue==GpioPinValue.High)
            {
                PiController.TurnOnLED();
            }
            else if (PiController.pinValue == GpioPinValue.Low)
            {
                PiController.TurnOffLED();
            }
        }
    }
}
