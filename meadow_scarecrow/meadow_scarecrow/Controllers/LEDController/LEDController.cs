using System;
using System.Threading.Tasks;
using System.Threading;

using Meadow.Foundation.Leds;
using Meadow.Foundation;
using Meadow.Devices;

namespace meadow_scarecrow.Controllers.LEDController
{
    internal partial class LEDController : InitalizedBaseController
    {
        RgbPwmLed onBoardRGBLed;

        Task animationTask = null;
        CancellationTokenSource cancellationTokenSource = null;

        public static LEDController Current { get; private set; }

        private LEDController() { }

        static LEDController()
        {
            Current = new LEDController();
        }

        public override Task Initialize()
        {
            if (initialized)
            {
                return Task.CompletedTask;
            }

            onBoardRGBLed = new RgbPwmLed(
                redPwmPin: MeadowApp.Device.Pins.OnboardLedRed,
                greenPwmPin: MeadowApp.Device.Pins.OnboardLedGreen,
                bluePwmPin: MeadowApp.Device.Pins.OnboardLedBlue,
                commonType: Meadow.Peripherals.Leds.CommonType.CommonAnode);
            onBoardRGBLed.SetColor(Color.Yellow);

            initialized = true;

            return base.Initialize();
        }

        void Stop()
        {
            onBoardRGBLed.StopAnimation();
            cancellationTokenSource?.Cancel();
        }

        public void SetColor(Color color)
        {
            Stop();
            onBoardRGBLed.SetColor(color);
        }

        public void TurnOn()
        {
            Stop();
            onBoardRGBLed.SetColor(GetRandomColor());
            onBoardRGBLed.IsOn = true;
        }

        public void TurnOff()
        {
            Stop();
            onBoardRGBLed.IsOn = false;
        }

        public void StartBlink()
        {
            Stop();
            onBoardRGBLed.StartBlink(GetRandomColor());
        }

        public void StartPulse()
        {
            Stop();
            onBoardRGBLed.StartPulse(GetRandomColor());
        }

        public void StartRunningColors()
        {
            onBoardRGBLed.StopAnimation();

            animationTask = new Task(async () =>
            {
                cancellationTokenSource = new CancellationTokenSource();
                await StartRunningColors(cancellationTokenSource.Token);
            });
            animationTask.Start();
        }

        protected async Task StartRunningColors(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                onBoardRGBLed.SetColor(GetRandomColor());
                await Task.Delay(1000);
            }
        }

        protected Color GetRandomColor()
        {
            var random = new Random();
            return Color.FromHsba(random.NextDouble(), 1, 1);
        }
    }
}
