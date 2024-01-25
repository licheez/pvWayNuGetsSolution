namespace pvWay.agentPoolManager.nc6;

internal abstract class BaseAgent : IAgent
{
    private readonly Action<IAgent> _endAction;
    protected readonly TimeSpan SleepSpan;
    private readonly AgentHandler _agentHandler = new();

    public Guid Id { get; }
    public DateTime StartTimeUtc { get; }
    public string Title { get; }

    protected BaseAgent(
        Action<IAgent> endAction,
        string title,
        TimeSpan sleepSpan)
    {
        _endAction = endAction;
        SleepSpan = sleepSpan;

        Id = Guid.NewGuid();
        StartTimeUtc = DateTime.UtcNow;
        Title = title;
    }

    public void RequestToStop()
    {
        lock (_agentHandler)
        {
            _agentHandler.SetIsStopRequested();
        }
    }

    protected void SetIsRunning()
    {
        lock (_agentHandler)
        {
            _agentHandler.SetIsRunning();
        }
    }

    protected bool IsStopRequested
    {
        get
        {
            lock (_agentHandler)
            {
                return _agentHandler.IsStopRequested;
            }
        }
    }

    protected void Stop()
    {
        lock (_agentHandler)
        {
            _agentHandler.SetIsStopped();
            _endAction(this);
        }
    }
}

internal class Agent : BaseAgent
{
    private readonly Action _repeat;

    public Agent(
        Action<IAgent> endAction,
        string title,
        Action repeat,
        TimeSpan sleepSpan,
        ThreadPriority priority = ThreadPriority.Normal) :
        base(endAction, title, sleepSpan)
    {
        _repeat = repeat;
        var asyncWorker = new Thread(AsyncWorker)
        {
            Priority = priority
        };
        asyncWorker.Start();
    }

    private void AsyncWorker()
    {
        SetIsRunning();
        while (true)
        {
            if (IsStopRequested)
            {
                Stop();
                break;
            }
            _repeat();
            var repeatUtc = DateTime.UtcNow.Add(SleepSpan);
            while (!IsStopRequested 
                   && DateTime.UtcNow < repeatUtc)
                Thread.Sleep(100);
        }
    }
}

internal class Agent<T> : BaseAgent
{
    private readonly Action<T> _repeat;

    public Agent(
        Action<IAgent> endAction,
        string title,
        Action<T> repeat,
        T workerParam,
        TimeSpan sleepSpan,
        ThreadPriority priority = ThreadPriority.Normal) :
        base(endAction, title, sleepSpan)
    {
        _repeat = repeat;
        var asyncWorker = new Thread(AsyncWorker)
        {
            Priority = priority
        };
        asyncWorker.Start(workerParam);
    }

    private void AsyncWorker(object? workerParam)
    {
        SetIsRunning();
        while (true)
        {
            if (IsStopRequested)
            {
                Stop();
                break;
            }
                
            var tParam = (T)workerParam!;
            _repeat(tParam);

            var repeatUtc = DateTime.UtcNow.Add(SleepSpan);
            while (!IsStopRequested 
                   && DateTime.UtcNow < repeatUtc)
                Thread.Sleep(100);
        }
    }
}