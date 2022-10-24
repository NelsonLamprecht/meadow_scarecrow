using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Meadow.Foundation.Web.Maple.Server.Routing;
using Meadow.Foundation.Web.Maple.Server;

using meadow_scarecrow.Controllers;
using Meadow.Foundation;

namespace meadow_scarecrow
{
    public class ControllerRequestHandler : RequestHandlerBase
    {
        public ControllerRequestHandler() { }

        public override bool IsReusable => true;

        [HttpPost("/up")]
        public async Task<IActionResult> UpAsync()
        {
            try
            {
                LedController.Current.SetColor(Color.Yellow);
                Task t = new Task(async () =>
                {
                    RelayController.Current.TurnOn();
                    await Task.Delay(250);
                    LedController.Current.SetColor(Color.Green);
                });
                t.Start();
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return new OkResult();
        }

        [HttpPost("/down")]
        public async Task<IActionResult> DownAsync()
        {
            try
            {
                LedController.Current.SetColor(Color.Yellow);
                Task t = new Task(async () =>
                {
                    RelayController.Current.TurnOff();
                    await Task.Delay(250);
                    LedController.Current.SetColor(Color.Green);
                });
                t.Start();
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return new OkResult();
        }
    }
}
