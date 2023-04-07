﻿namespace pvWay.agentPoolManager.nc6
{
    internal class AgentHandler
    {
        private bool CanStart => !IsRunning && !IsStopRequested;
        private bool IsRunning { get; set; }
        public bool IsStopRequested { get; private set; }

        //public int ItemCount { get; private set; }
        //public int CurrentIndex { get; private set; }
        //public string Status { get; private set; }

        public void SetIsRunning()
        {
            if (!CanStart) return;
            IsRunning = true;
            IsStopRequested = false;
        }

        public void SetIsStopped()
        {
            if (!IsRunning) return;
            IsRunning = false;
            IsStopRequested = false;
        }

        public void SetIsStopRequested()
        {
            if (!IsRunning || IsStopRequested) return;
            IsStopRequested = true;
        }

    }
}
