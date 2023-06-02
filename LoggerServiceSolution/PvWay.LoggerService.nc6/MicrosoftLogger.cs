using Microsoft.Extensions.Logging;

namespace PvWay.LoggerService.nc6
{
    internal class MicrosoftLogger : Logger
    {
        public MicrosoftLogger(
            ILogger msLogger) : base(
            new MicrosoftLoggerWriter(msLogger))
        {
        }
    }
}
