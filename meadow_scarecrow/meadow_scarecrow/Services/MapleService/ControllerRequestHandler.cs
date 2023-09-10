//using System;
//using System.Diagnostics;
//using System.Threading.Tasks;

//using Meadow.Foundation;
//using Meadow.Foundation.Web.Maple;
//using Meadow.Foundation.Web.Maple.Routing;

//using meadow_scarecrow.Controllers;

//namespace meadow_scarecrow
//{
//    public class ControllerRequestHandler : RequestHandlerBase
//    {
//        public ControllerRequestHandler() { }

//        public override bool IsReusable => true;

//        [HttpPost("/up")]
//        public async Task<IActionResult> UpAsync()
//        {
//            try
//            {
//                Task t = new Task(async () =>
//                {
                    
//                    await Task.Delay(250);
//                });
//                t.Start();
//                await Task.CompletedTask;
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(ex);
//            }
//            return new OkResult();
//        }

//        [HttpPost("/down")]
//        public async Task<IActionResult> DownAsync()
//        {
//            try
//            {                
//                Task t = new Task(async () =>
//                {
//                    RelayController.Current.TurnOff();
//                    await Task.Delay(250);
//                });
//                t.Start();
//                await Task.CompletedTask;
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(ex);
//            }
//            return new OkResult();
//        }
//    }
//}
