using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace IoTApp.Sensors
{
    public class PiController
    {
        public static GpioController GPIOController;
        //static GpioPin GPIOInputPin;
        static GpioPin GPIOOutputPin;
        public static GpioPinValue pinValue;
        const int INPUTPIN = 16;
        const int OUTPUTPIN = 4;

        public static bool InitializeGPIO()
        {
            bool status = false;
            try
            {
                GPIOController = GpioController.GetDefault();

                if (GPIOController == null)
                {
                    return false;
                }

                //GPIOInputPin = GPIOController.OpenPin(INPUTPIN);
                //GPIOInputPin.SetDriveMode(GpioPinDriveMode.Input);

                GPIOOutputPin = GPIOController.OpenPin(OUTPUTPIN);
                GPIOOutputPin.SetDriveMode(GpioPinDriveMode.Output);
                pinValue = GpioPinValue.High;
                GPIOOutputPin.Write(pinValue);

                return true;
            }
            catch (Exception)
            {
                //log error
                status = false;
            }
            return status;
        }

        public static void TurnOffLED()
        {
            try
            {
                pinValue = GpioPinValue.Low;
                GPIOOutputPin.Write(pinValue);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public static void TurnOnLED()
        {
            try
            {
                pinValue = GpioPinValue.High;
                GPIOOutputPin.Write(pinValue);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
