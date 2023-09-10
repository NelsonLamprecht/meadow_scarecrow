using System.Threading.Tasks;

using Meadow;
using Meadow.Foundation.Web.Maple;
using Meadow.Hardware;
using Meadow.Logging;

namespace meadow_scarecrow.Services.MapleService
{
    internal class MapleService: BaseService
    {
        private readonly MapleServer mapleServer;

        public MapleService(Logger logger, IMeadowDevice device, INetworkAdapter networkAdapter ) : base(logger)
        {

            mapleServer = new MapleServer(networkAdapter.IpAddress, advertise: true, processMode: RequestProcessMode.Parallel)
            {
                AdvertiseIntervalMs = 1500, // every 1.5 seconds
                DeviceName = device.Information.DeviceName
            };
        }

        public override Task Run()
        {
            mapleServer.Start();
            return base.Run();
        }
    }
}
