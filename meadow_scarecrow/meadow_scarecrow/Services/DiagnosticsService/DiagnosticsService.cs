using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Hardware;
using Meadow.Logging;
using meadow_scarecrow.Controllers.LEDController;

namespace meadow_scarecrow.Services.DiagnosticsService
{
    internal class DiagnosticsService : BaseService
    {
        private const string outputFooter = "=======================================================================";

        private readonly IMeadowDevice device;
        private readonly INetworkAdapter networkAdapter;
        private readonly ILEDDeviceController ledDevice;

        public DiagnosticsService(Logger logger, IMeadowDevice device, INetworkAdapter wifi, ILEDDeviceController ledDevice) : base(logger)
        {
            this.device = device;
            this.networkAdapter = wifi;
            this.ledDevice = ledDevice;
        }

        public void OutputMeadowOSInfo()
        {
            Logger.Info($"=========================OutputMeadowOSInfo============================");
            Logger.Info($"OS version: {MeadowOS.SystemInformation.OSVersion}");
            Logger.Info($"Runtime version: {MeadowOS.SystemInformation.RuntimeVersion}");
            Logger.Info($"Build date: {MeadowOS.SystemInformation.OSBuildDate}");
            Logger.Info(outputFooter);
        }

        public void OutputDeviceInfo()
        {
            Logger.Info($"=========================OutputDeviceInfo==============================");
            Logger.Info($"Device name: {device.Information.DeviceName}");
            Logger.Info($"Processor serial number: {device.Information.ProcessorSerialNumber}");
            Logger.Info($"Processor ID: {device.Information.UniqueID}");
            Logger.Info($"Model: {device.Information.Model}");
            Logger.Info($"Processor type: {device.Information.ProcessorType}");
            Logger.Info($"Product: {device.Information.Model}");
            Logger.Info($"Coprocessor type: {device.Information.CoprocessorType}");
            Logger.Info($"Coprocessor firmware version: {device.Information.CoprocessorOSVersion}");
            Logger.Info(outputFooter);
        }

        public void OutputNtpInfo()
        {
            Logger.Info($"=========================OutputMeadowOSInfo============================");
            Logger.Info($"NTP Client Enabled: {device.PlatformOS.NtpClient.Enabled}");
            Logger.Info(outputFooter);
        }

        public void OutputDeviceWifiInfo()
        {
            var isF7PlatformOS = device.PlatformOS is F7PlatformOS;
            if (isF7PlatformOS && networkAdapter is Esp32Coprocessor esp32Wifi)
            {
                Logger.Info($"====================OutputDeviceWifiInfo===========================");
                Logger.Info($"DefaultSsid: {esp32Wifi.DefaultSsid}");
                Logger.Info($"MacAddress: {esp32Wifi.MacAddress}");
                Logger.Info($"IpAddress: {esp32Wifi.IpAddress}");
            }

            Logger.Info(outputFooter);
        }
    }
}
