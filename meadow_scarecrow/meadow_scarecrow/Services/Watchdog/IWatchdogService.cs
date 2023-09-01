using System;

namespace meadow_scarecrow.Services.Watchdog
{
    internal interface IWatchdogService: IRunableService
    {
        void EnableWatchdog(int watchdogInSeconds);

        void PetWatchdog(int watchdogInSeconds);
    }
}