using Meadow.Gateways;

namespace meadow_scarecrow
{
    public class AppConfigRoot
    {
        public Network Network { get; set; }
    }

    public class Network
    {
        public Wifi Wifi { get; set; }
    }

    public class Wifi
    {
        public string SSID { get; set; }

        public string Password { get; set; }

        public string AntennaType { get; set; }
    }
}
