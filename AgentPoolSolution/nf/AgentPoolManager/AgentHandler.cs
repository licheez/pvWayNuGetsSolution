namespace pvWay.AgentPoolManager
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

        //public double Progress => ItemCount == 0
        //    ? 0
        //    : CurrentIndex / ItemCount;

        //public void SetItemCount(int itemCount)
        //{
        //    ItemCount = itemCount;
        //}

        //public void SetCurrentIndex(int index)
        //{
        //    CurrentIndex = index;
        //}

        //public void SetStatus(string status)
        //{
        //    Status = status;
        //}

        //public int IncrementCurrentIndex(int i)
        //{
        //    CurrentIndex += i;
        //    return CurrentIndex;
        //}
    }
}
