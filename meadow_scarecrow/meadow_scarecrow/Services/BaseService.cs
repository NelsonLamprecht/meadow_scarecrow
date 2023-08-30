using Meadow.Logging;
using System.Threading.Tasks;

namespace meadow_scarecrow.Services
{
    internal class BaseService
    {
        public BaseService(Logger logger)
        {
            Logger = logger;
        }

        public Logger Logger { get; }

        public virtual Task Run()
        {
            return Task.CompletedTask;
        }

    }
}