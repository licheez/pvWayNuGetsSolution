﻿namespace PvWay.LoggerService.nc6
{
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger() :
            base(new ConsoleLogWriter())
        {
        }
    }
}
