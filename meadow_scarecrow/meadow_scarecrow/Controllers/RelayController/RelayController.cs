﻿using Meadow;
using Meadow.Foundation.Relays;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Peripherals.Relays;

namespace meadow_scarecrow.Controllers.RelayController
{
    // <summary>
    // The commands all inverted since we are working with relays that keep the valves closed.
    // </summary>
    internal class RelayController : BaseController
    {
        private readonly IMeadowDevice device;
        private Relay relay;
        private bool initialized = false;

        public RelayController(Logger logger, IMeadowDevice device) : base(logger)
        {
            this.device = device;
        }

        public void Initialize(IPin devicePin)
        {
            if (initialized)
            {
                return;
            }
            var outputPort = device.CreateDigitalOutputPort(devicePin, true, OutputType.OpenDrain);
            relay = new Relay(outputPort, RelayType.NormallyOpen);

            // so port is closed as quickly as possible when board boots up
            TurnOff();
            initialized = true;
        }

        public void TurnOff()
        {
            relay.IsOn = !false;
            Logger.Debug("Relay Is Off.");
        }

        public void TurnOn()
        {
            relay.IsOn = !true;
            Logger.Debug("Relay Is On.");
        }
    }
}