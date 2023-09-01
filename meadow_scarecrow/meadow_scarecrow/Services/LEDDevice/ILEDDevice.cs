using Meadow.Foundation;

namespace meadow_scarecrow.Services.LEDDevice
{
    public interface ILEDDevice
    {
        void SetColor(Color color);

        void Stop();
    }
}
