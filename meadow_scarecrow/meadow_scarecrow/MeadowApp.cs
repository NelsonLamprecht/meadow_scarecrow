using System;
using System.Threading.Tasks;

using Meadow;
using Meadow.Foundation;
using Meadow.Hardware;

using meadow_scarecrow.Services.DiagnosticsService;
using meadow_scarecrow.Services.LEDDevice;
using meadow_scarecrow.Services.Watchdog;

namespace meadow_scarecrow
{
    public class MeadowApp : MeadowBase
    {
        private IWiFiNetworkAdapter _wifiAdapter;
        private ILEDDevice _ledDevice;
        private DiagnosticsService _diagnostics;
        
        public override async Task Initialize()
        {
            var network = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            network.SetAntenna(AntennaType.External);

            Resolver.Services.Add(network);

            Resolver.Services.Create<OnBoardLEDDevice, ILEDDevice>();
            Resolver.Services.Create<WatchdogService, IWatchdogService>();

            // when succesfull, this sets LED to green
            Resolver.Services.Create<DiagnosticsService>();

            ////RelayController.Current.Initialize(Device.CreateDigitalOutputPort(Device.Pins.D05, true, OutputType.OpenDrain), RelayType.NormallyOpen);

            await base.Initialize();
        }

        public override async Task Run()
        {
            try
            {
                Logger.Debug($"+Run");

                _diagnostics = Services.Get<DiagnosticsService>();
                _diagnostics.OutputDeviceInfo();
                _diagnostics.OutputMeadowOSInfo();
                _diagnostics.OutputNtpInfo();

                var w = Services.Get<IWatchdogService>();
                w.EnableWatchdog(15);
                w.PetWatchdog(10);
                _ledDevice.SetColor(Color.Blue);

                _wifiAdapter = Services.Get<IWiFiNetworkAdapter>();
                // this will set led to green to indicate ready to go
                _wifiAdapter.NetworkConnected += _diagnostics.NetworkConnected;
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