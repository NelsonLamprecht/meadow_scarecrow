using Meadow.Foundation;

namespace meadow_scarecrow.Controllers.LEDController
{
    public interface ILEDDeviceController
    {
        void SetColor(Color color);

        void Stop();

        void StartBlink(Color color);

        void TurnOn();

        void TurnOff();
    }
}
