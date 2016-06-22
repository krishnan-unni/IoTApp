using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace IoTApp
{
    public class PiController
    {
        public static GpioController GPIOController;
        static GpioPin GPIOInputPin;
        static GpioPin GPIOOutputPin;

        const int INPUTPIN = 16;
        const int OUTPUTPIN = 17;

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

                GPIOInputPin = GPIOController.OpenPin(INPUTPIN);
                GPIOInputPin.SetDriveMode(GpioPinDriveMode.Input);

                GPIOOutputPin = GPIOController.OpenPin(OUTPUTPIN);
                GPIOOutputPin.SetDriveMode(GpioPinDriveMode.Output);

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
                GPIOOutputPin.Write(GpioPinValue.Low);
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
                GPIOOutputPin.Write(GpioPinValue.High);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
