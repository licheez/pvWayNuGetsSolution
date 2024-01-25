using PvWay.AgentPoolManager.Abstraction.nc8;

namespace PvWay.AgentPoolManager.nc8.Impl;

internal abstract class BaseAgent(
    Action<IPvWayAgentPoolManagerAgent> endAction,
    string title,
    TimeSpan sleepSpan)
    : IPvWayAgentPoolManagerAgent
{
    protected readonly TimeSpan SleepSpan = sleepSpan;
    private readonly AgentHandler _agentHandler = new();

    public Guid Id { get; } = Guid.NewGuid();
    public DateTime StartTimeUtc { get; } = DateTime.UtcNow;
    public string Title { get; } = title;

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
            endAction(this);
        }
    }
}

internal class Agent : BaseAgent
{
    private readonly Action _repeat;

    public Agent(
        Action<IPvWayAgentPoolManagerAgent> endAction,
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
        Action<IPvWayAgentPoolManagerAgent> endAction,
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