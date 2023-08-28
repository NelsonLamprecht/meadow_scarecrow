using System;
using System.Threading.Tasks;

using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple;
using Meadow.Hardware;
using Meadow.Peripherals.Relays;

using meadow_scarecrow.Controllers;

namespace meadow_scarecrow
{
    public class MeadowApp : App<F7FeatherV1>
    {
        private IWiFiNetworkAdapter wifi;

        public override async Task Initialize()
        {
            Resolver.Log.Info("Initializing hardware...");

            // This sets up onboard rgb and exits with yellow
            await LedController.Current.Initialize();

            //RelayController.Current.Initialize(Device.CreateDigitalOutputPort(Device.Pins.D05, true, OutputType.OpenDrain), RelayType.NormallyOpen);

            wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            wifi.SetAntenna(AntennaType.External);
            
            if (wifi.IsConnected)
            {
                Resolver.Log.Info("WiFi adapter already connected.");
            }
            else
            {
                Resolver.Log.Info("WiFi adapter not connected.");
                wifi.NetworkConnected += Wifi_NetworkConnected;
            }

            await base.Initialize();
        }

        public override async Task Run()
        {
            try
            {
                Resolver.Log.Debug($"+Run");
                StartHeartbeat();

                OutputDeviceInfo();
                OutputNtpInfo();
                OutputMeadowOSInfo();
                OutputDeviceConfigurationInfo();

                LedController.Current?.SetColor(Color.Blue);
            }
            catch (Exception ex)
            {
                Resolver.Log.Error(ex.ToString());
                LedController.Current?.SetColor(Color.Red);
            }
            await base.Run();
        }

        protected void StartHeartbeat()
        {
            Resolver.Log.Debug($"+StartHeartbeat");

            Task.Run(async () =>
            {
                Resolver.Log.Trace($"Heartbeat Task Started");
                var countToReset = 1;

                while (true)
                {
                    Resolver.Log.Debug($"Count to reset: {countToReset}");
                    Resolver.Log.Info($"{DateTime.Now} {wifi.IpAddress}");
                    await Task.Delay(TimeSpan.FromSeconds(10));

                    Resolver.Log.Trace($"Testing for throw");
                    if (--countToReset <= 0) throw new Exception("Testing restart...");

                }
            });
        }

        private void ConfigureMapleServer()
        {
            Resolver.Log.Info("Initializing maple server...");
            
            var mapleServer = new MapleServer(wifi.IpAddress, advertise: true, processMode: RequestProcessMode.Parallel)
            {
                AdvertiseIntervalMs = 1500, // every 1.5 seconds
                DeviceName = Device.Information.DeviceName
            };
            mapleServer.Start();
        }

        void OutputDeviceInfo()
        {
            Resolver.Log.Info($"=========================OutputDeviceInfo==============================");
            Resolver.Log.Info($"Device name: {Device.Information.DeviceName}");
            Resolver.Log.Info($"Processor serial number: {Device.Information.ProcessorSerialNumber}");
            Resolver.Log.Info($"Processor ID: {Device.Information.UniqueID}");
            Resolver.Log.Info($"Model: {Device.Information.Model}");
            Resolver.Log.Info($"Processor type: {Device.Information.ProcessorType}");
            Resolver.Log.Info($"Product: {Device.Information.Model}");
            Resolver.Log.Info($"Coprocessor type: {Device.Information.CoprocessorType}");
            Resolver.Log.Info($"Coprocessor firmware version: {Device.Information.CoprocessorOSVersion}");
            Resolver.Log.Info($"=======================================================================");
        }

        void OutputNtpInfo()
        {
            Resolver.Log.Info($"=========================OutputMeadowOSInfo============================");
            Resolver.Log.Info($"NTP Client Enabled: {Device.PlatformOS.NtpClient.Enabled}");
            Resolver.Log.Info($"=======================================================================");
        }

        void OutputMeadowOSInfo()
        {
            Resolver.Log.Info($"=========================OutputMeadowOSInfo============================");
            Resolver.Log.Info($"OS version: {MeadowOS.SystemInformation.OSVersion}");
            Resolver.Log.Info($"Runtime version: {MeadowOS.SystemInformation.RuntimeVersion}");
            Resolver.Log.Info($"Build date: {MeadowOS.SystemInformation.OSBuildDate}");
            Resolver.Log.Info($"=======================================================================");
        }

        void OutputDeviceConfigurationInfo()
        {
            try
            {
                // Retrieve
                var isF7PlatformOS = Device.PlatformOS is F7PlatformOS;
                var esp32Wifi = wifi as Esp32Coprocessor;
                if (isF7PlatformOS && esp32Wifi != null)
                {
                    Resolver.Log.Info($"====================OutputDeviceConfigurationInfo======================");
                    Resolver.Log.Info($"Automatically connect to network: {F7PlatformOS.GetBoolean(IPlatformOS.ConfigurationValues.AutomaticallyStartNetwork)}");
                    Resolver.Log.Info($"Get time at startup: {F7PlatformOS.GetBoolean(IPlatformOS.ConfigurationValues.GetTimeAtStartup)}");
                    Resolver.Log.Info($"Default access point: {F7PlatformOS.GetString(IPlatformOS.ConfigurationValues.DefaultAccessPoint)}");
                    // Note: You can also access the maximum retry count via the ESP32 coprocessor using `esp32Wifi.MaximumRetryCount`.
                    Resolver.Log.Info($"Maximum retry count: {F7PlatformOS.GetUInt(IPlatformOS.ConfigurationValues.MaximumNetworkRetryCount)}");
                    Resolver.Log.Info($"=======================================================================");
                }
            }
            catch (Exception e)
            {
                Resolver.Log.Error(e.Message);
            }
        }

        void OutputDeviceWifiInfo()
        {
            var isF7PlatformOS = Device.PlatformOS is F7PlatformOS;
            var esp32Wifi = wifi as Esp32Coprocessor;
            Resolver.Log.Info($"====================OutputDeviceWifiInfo======================");

            if (isF7PlatformOS && esp32Wifi != null)
            {
                Resolver.Log.Info($"DefaultSsid: {esp32Wifi.DefaultSsid}");
                Resolver.Log.Info($"MacAddress: {esp32Wifi.MacAddress}");
                Resolver.Log.Info($"IpAddress: {esp32Wifi.IpAddress}");
            }

            Resolver.Log.Info($"==============================================================");
        }        

        private void Wifi_NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            OutputDeviceWifiInfo();
            LedController.Current.SetColor(Color.Purple);

            ConfigureMapleServer();

            LedController.Current?.SetColor(Color.Green);
        }

    }
}