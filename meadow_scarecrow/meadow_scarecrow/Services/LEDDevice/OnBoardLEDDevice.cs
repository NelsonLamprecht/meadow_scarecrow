using System.Threading;

using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Leds;
using Meadow.Foundation;

namespace meadow_scarecrow.Services.LEDDevice
{

    public class OnBoardLEDDevice : ILEDDevice
    {
        CancellationTokenSource cancellationTokenSource = null;
        private readonly IMeadowDevice _device;
        private readonly RgbPwmLed _rgbLed;

        public OnBoardLEDDevice(IMeadowDevice device)
        {
            this._device = device;

            // TODO: handle other devices
            if (_device is F7FeatherV1 f7DeviceV1)
            {
                _rgbLed = new RgbPwmLed(
                    redPwmPin: f7DeviceV1.Pins.OnboardLedRed,
                    greenPwmPin: f7DeviceV1.Pins.OnboardLedGreen,
                    bluePwmPin: f7DeviceV1.Pins.OnboardLedBlue,
                    commonType: Meadow.Peripherals.Leds.CommonType.CommonAnode);
            }

            _rgbLed.SetColor(Color.White);
        }

        public void SetColor(Color color)
        {
            Stop();
            _rgbLed?.SetColor(color);
        }

        public void Stop()
        {
            _rgbLed?.StopAnimation();
            cancellationTokenSource?.Cancel();
        }
    }
}

