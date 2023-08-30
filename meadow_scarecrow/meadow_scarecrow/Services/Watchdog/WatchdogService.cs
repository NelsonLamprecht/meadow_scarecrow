using Meadow;
using Meadow.Gateways.Bluetooth;
using Meadow.Hardware;
using Meadow.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace meadow_scarecrow.Services.Watchdog
{
    internal class WatchdogService : BaseService, IWatchdogService
    {
        private readonly IMeadowDevice device;
        private int watchdogInSeconds;

        public WatchdogService(Logger logger, IMeadowDevice device) : base(logger)
        {
            this.device = device;
        }        

        public void EnableWatchdog(int watchdogInSeconds)
        {
            this.watchdogInSeconds = watchdogInSeconds;
            device.WatchdogEnable(TimeSpan.FromSeconds(watchdogInSeconds));
        }

        public void PetWatchdog(int watchDogPettingInSeconds)
        {
            // Just for good measure, let's reset the watchdog to begin with.
            device.WatchdogReset();
            // Start a thread that restarts it.
            Thread t = new Thread(() => {
                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(watchDogPettingInSeconds));
                    Console.WriteLine("Petting watchdog.");
                    device.WatchdogReset();
                }
            });
            t.Start();
        }
    }
}
