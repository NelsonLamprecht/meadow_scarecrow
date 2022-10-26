using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using Meadow.Peripherals.Relays;

using meadow_scarecrow.Controllers;

namespace meadow_scarecrow
{
    public class MeadowApp : App<F7FeatherV1>
    {
        private const string appConfigFileName = "app.config.json";
        

        public override Task Initialize()
        {
            Console.WriteLine("Initializing hardware...");
            Device.Information.DeviceName = "MeadowF7FeatherV1-Scarecrow";

            LedController.Current.Initialize();
            RelayController.Current.Initialize(Device.CreateDigitalOutputPort(Device.Pins.D05, true, OutputType.OpenDrain), RelayType.NormallyOpen);

            return base.Initialize();
        }

        public override async Task Run()
        {
            try
            {
                await ConfigureWiFi();
                LedController.Current.SetColor(Color.Blue);
                ConfigureMapleServer();
                LedController.Current.SetColor(Color.Green);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                LedController.Current.SetColor(Color.Red);
            }
        }

        private void ConfigureMapleServer()
        {
            Console.WriteLine("Initializing hardware...");
            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            
            var mapleServer = new MapleServer(wifi.IpAddress, advertise: true, processMode: RequestProcessMode.Parallel)
            {
                AdvertiseIntervalMs = 1500, // every 1.5 seconds
                DeviceName = Device.Information.DeviceName
            };
            mapleServer.Start();
        }

        private async Task ConfigureWiFi()
        {
            Console.WriteLine("Configuring Wifi...");
            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

            await ScanForAccessPoints(wifi);

            AppConfigRoot appConfigRoot = await GetAppConfig();

            if (appConfigRoot != null
                &&
                appConfigRoot.Network != null
                &&
                appConfigRoot.Network.Wifi != null
                &&
                appConfigRoot.Network.Wifi.SSID != null
                &&
                appConfigRoot.Network.Wifi.Password != null)
            {
                

                //if (Enum.TryParse<AntennaType>(appConfigRoot.Network.Wifi.AntennaType, out var antennaType))
                //{
                //    if (antennaType == AntennaType.External)
                //    {
                //        wifi.SetAntenna(AntennaType.External);
                //    }
                //    if (antennaType == AntennaType.OnBoard)
                //    {
                //        wifi.SetAntenna(AntennaType.OnBoard);
                //    }
                //}
                
                ConnectionResult connectionResult = await wifi.Connect(appConfigRoot.Network.Wifi.SSID, appConfigRoot.Network.Wifi.Password);
                if (connectionResult.ConnectionStatus != ConnectionStatus.Success)
                {
                    LedController.Current.SetColor(Color.Red);
                    throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
                }

                Console.WriteLine(wifi.IpAddress);
            }
            else
            {
                LedController.Current.SetColor(Color.Red);
                throw new Exception("Unable to get network configuration from file.");
            }
        }

        private async Task<AppConfigRoot> GetAppConfig()
        {
            var appConfigFilePath = Path.Combine(MeadowOS.FileSystem.UserFileSystemRoot, appConfigFileName);
            var appConfig = await GetFileContentsAsync<AppConfigRoot>(appConfigFilePath);
            if (appConfig != default)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("Network:");
                Console.WriteLine($"\tSSID: {appConfig.Network.Wifi.SSID}");
                Console.WriteLine($"\tPassword: {appConfig.Network.Wifi.Password}");
            }

            return appConfig;
        }

        private async Task<T> GetFileContentsAsync<T>(string path)
        {
            Console.WriteLine($"\tFile: {Path.GetFullPath(path)} ");
            try
            {
                using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    var fileContents = await streamReader.ReadToEndAsync();
                    var result = JsonSerializer.Deserialize<T>(fileContents);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return default;
        }

        async Task ScanForAccessPoints(IWiFiNetworkAdapter wifi)
        {
            Console.WriteLine("Getting list of access points.");
            var networks = await wifi.Scan(TimeSpan.FromSeconds(60));

            if (networks.Count > 0)
            {
                Console.WriteLine("|-------------------------------------------------------------|---------|");
                Console.WriteLine("|         Network Name             | RSSI |       BSSID       | Channel |");
                Console.WriteLine("|-------------------------------------------------------------|---------|");

                foreach (WifiNetwork accessPoint in networks)
                {
                    Console.WriteLine($"| {accessPoint.Ssid,-32} | {accessPoint.SignalDbStrength,4} | {accessPoint.Bssid,17} |   {accessPoint.ChannelCenterFrequency,3}   |");
                }
            }
            else
            {
                Console.WriteLine($"No access points detected.");
            }
        }
    }
}