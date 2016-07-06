using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Pwm;
using Windows.UI;

namespace IoTApp.Sensors
{
    public class RGBLedController : IDisposable
    {
        PwmPin[] _pins;

        public RGBLedController(PwmPin Rpin, PwmPin Gpin, PwmPin Bpin)
        {
            _pins = new PwmPin[] { Rpin, Gpin, Bpin };
        }

        public void On()
        {
            foreach (var pin in _pins)
            {
                if (!pin.IsStarted)
                {
                    pin.Start();
                }
            }
        }

        public void Off()
        {
            foreach (var pin in _pins)
            {
                if (pin.IsStarted)
                {
                    pin.Stop();
                }
            }
        }

        private Color _color;
        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
                UpdateLed();
            }
        }

        private void UpdateLed()
        {
            double[] colorValue = new double[] { Color.R, Color.G, Color.B };
            for (int i = 0; i < colorValue.Length; i++)
            {
                colorValue[i] /= 255;
                _pins[i].SetActiveDutyCyclePercentage(colorValue[i]);
            }
        }

        public void Dispose()
        {
            if (_pins != null)
            {
                foreach (var pin in _pins)
                {
                    if (pin.IsStarted)
                    {
                        pin.Stop();
                    }
                    pin.Dispose();
                }
                _pins = null;
            }
        }
    }
}
