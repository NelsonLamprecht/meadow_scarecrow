using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Hardware;
using Meadow.Logging;

using meadow_scarecrow.Controllers.LEDController;

namespace meadow_scarecrow.Services.NetworkService
{
    internal class NetworkService: BaseService
    {
        private readonly IMeadowDevice meadowDevice;
        private readonly ILEDDeviceController ledDevice;
        private readonly DiagnosticsService.DiagnosticsService diagnosticsService;

        public NetworkService(Logger logger, IMeadowDevice meadowDevice, ILEDDeviceController ledDevice, DiagnosticsService.DiagnosticsService diagnosticsService) : base(logger)
        {
            this.meadowDevice = meadowDevice;
            this.ledDevice = ledDevice;
            this.diagnosticsService = diagnosticsService;
        }

        public void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            var isF7PlatformOS = meadowDevice.PlatformOS is F7PlatformOS;
            var isEspCoprocessor = sender is Esp32Coprocessor esp32Wifi;
            if (isF7PlatformOS && isEspCoprocessor)
            {
                diagnosticsService.OutputDeviceWifiInfo();
                ledDevice.StartBlink(Color.Green);
            }
            
        }
    }
}
