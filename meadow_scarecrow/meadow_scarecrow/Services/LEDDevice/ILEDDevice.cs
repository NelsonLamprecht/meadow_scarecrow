using Meadow.Foundation;
using Meadow.Foundation.Leds;

namespace meadow_scarecrow.Services.LEDDevice
{
    public interface ILEDDevice
    {
        void SetColor(Color color);

        void Stop();

        void StartBlink(Color color);

        void TurnOn();

        void TurnOff();
    }
}
