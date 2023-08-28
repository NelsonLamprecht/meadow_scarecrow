using System;

using Meadow.Foundation.Relays;
using Meadow.Hardware;
using Meadow.Peripherals.Relays;

namespace meadow_scarecrow.Controllers
{
    /// <summary>
    /// The commands all inverted since the pneumatics are keeping the values closed
    /// </summary>
    //internal class RelayController: InitalizedBaseController
    //{
    //    private Relay _relay;
    //    private bool _debug = false;

    //    public static RelayController Current
    //    {
    //        get;
    //        private set;
    //    }

    //    static RelayController()
    //    {
    //        Current = new RelayController();
    //    }

    //    private RelayController() {  }

    //    public void Initialize(IDigitalOutputPort digitalOutputPort, RelayType relayType)
    //    {
    //        if (initialized)
    //        {
    //            return;
    //        }
    //        // true so port is closed as quickly as possible when board boots up
    //        _relay = new Relay(digitalOutputPort, relayType);
    //        TurnOff();
    //        initialized = true;

    //        base.Initialize();
    //    }

    //    public void DebugOff()
    //    {
    //        _debug = false;
    //    }

    //    public void DebugOn()
    //    {
    //        _debug = true;
    //    }

    //    public void TurnOff()
    //    {
    //        _relay.IsOn = !false;
    //        if (_debug)
    //        {
    //            Resolver.Log.Info("Relay Is Off.");
    //        }
    //    }

    //    public void TurnOn()
    //    {
    //        _relay.IsOn = !true;
    //        if(_debug)
    //        {
    //            Resolver.Log.Info("Relay Is On.");
    //        }
    //    }
    //}
}