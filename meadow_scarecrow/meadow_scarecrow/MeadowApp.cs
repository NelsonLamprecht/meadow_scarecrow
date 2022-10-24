using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Gateway.WiFi;
using Meadow.Gateways;
using Meadow.Hardware;
using Meadow.Peripherals.Relays;
using meadow_scarecrow.Controllers;

namespace meadow_scarecrow
{
    public class MeadowApp : App<F7FeatherV1, MeadowApp>
    {
        private const string appConfigFileName = "app.config.json";
        private MapleServer _mapleServer;
       
        public MeadowApp()
        {
            try
            {
                Device.Information.DeviceName = "MeadowF7v1-Scarecrow";
                Initialize().Wait();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task Initialize()
        {
            Console.WriteLine("Initializing hardware...");
            LedController.Current.Initialize();
            RelayController.Current.Initialize(Device.CreateDigitalOutputPort(Device.Pins.D05, true, OutputType.OpenDrain), RelayType.NormallyOpen);

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
                if (Enum.TryParse<AntennaType>(appConfigRoot.Network.Wifi.AntennaType, out var antennaType))
                {
                    if (antennaType == AntennaType.OnBoard)
                    {
                        Device.SetAntenna(AntennaType.OnBoard);
                    }
                    else if (antennaType == AntennaType.External)
                    {
                        Device.SetAntenna(AntennaType.OnBoard);
                    }
                }
                
                ConnectionResult connectionResult = await Device.WiFiAdapter.Connect(appConfigRoot.Network.Wifi.SSID, appConfigRoot.Network.Wifi.Password, TimeSpan.FromSeconds(60));
                if (connectionResult.ConnectionStatus != ConnectionStatus.Success)
                {
                    LedController.Current.SetColor(Color.Red);
                    throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
                }

                _mapleServer = new MapleServer(Device.WiFiAdapter.IpAddress, 5417, true, RequestProcessMode.Serial)
                {
                    AdvertiseIntervalMs = 1500, // every 1.5 seconds
                    DeviceName = Device.Information.DeviceName
                };
                _mapleServer.Start();

                LedController.Current.SetColor(Color.Green);
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
    }
}