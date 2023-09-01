using System;
using System.Threading.Tasks;

using Meadow.Foundation;
using Meadow.Hardware;
using meadow_scarecrow.Controllers.LEDController;
using meadow_scarecrow.Controllers.RelayController;
using meadow_scarecrow.Services.DiagnosticsService;
using meadow_scarecrow.Services.Watchdog;

namespace meadow_scarecrow
{
    public class MeadowApp : MeadowBase
    {
        private ILEDDeviceController _ledDevice;

        public override async Task Initialize()
        {
            var network = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            network.SetAntenna(AntennaType.External);

            Services.Add(network);

            //rework the device dependancy for the controller so its not just the meadow device
            _ledDevice = Services.Create<OnBoardLEDDeviceController, ILEDDeviceController>();
            Services.Create<WatchdogService, IWatchdogService>();

            Services.Create<DiagnosticsService>();
            Services.Create<RelayController>();

            await base.Initialize();
        }

        public override async Task Run()
        {
            try
            {
                Logger.Debug($"+Run");

                var diagnostics = Services.Get<DiagnosticsService>();
                diagnostics.OutputDeviceInfo();
                diagnostics.OutputMeadowOSInfo();
                diagnostics.OutputNtpInfo();

                var w = Services.Get<IWatchdogService>();
                w.Enable(15);
                w.Pet(10);

                var relayController = Services.Get<RelayController>();
                relayController.Initialize(Device.Pins.D05);
                _ledDevice.StartBlink(Color.Blue);

                var wifiAdapter = Services.Get<IWiFiNetworkAdapter>();
                // this will set led to green to indicate ready to go
                wifiAdapter.NetworkConnected += diagnostics.NetworkConnected;
            }

            catch (Exception ex)
            {
                _ledDevice.SetColor(Color.Red);
                Logger.Error(ex.ToString());
            }
            await base.Run();
        }

        //private void ConfigureMapleServer()
        //{
        //    Resolver.Log.Info("Initializing maple server...");

        //    var mapleServer = new MapleServer(wifi.IpAddress, advertise: true, processMode: RequestProcessMode.Parallel)
        //    {
        //        AdvertiseIntervalMs = 1500, // every 1.5 seconds
        //        DeviceName = Device.Information.DeviceName
        //    };
        //    mapleServer.Start();
        //}

    }
}