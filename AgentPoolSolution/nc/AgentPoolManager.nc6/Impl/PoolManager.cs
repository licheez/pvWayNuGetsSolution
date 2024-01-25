using pvWay.agentPoolManager.nc6.Abstractions;

namespace pvWay.agentPoolManager.nc6.Impl;

internal class PoolManager : IPvWayAgentPoolManager
{
    private readonly IDictionary<IPvWayAgentPoolManagerAgent, Action<IPvWayAgentPoolManagerAgent>?> _pool = 
        new Dictionary<IPvWayAgentPoolManagerAgent, Action<IPvWayAgentPoolManagerAgent>?>();
    public IEnumerable<IPvWayAgentPoolManagerAgent> Agents => _pool.Keys;

    public IPvWayAgentPoolManagerAgent? GetAgent(Guid id)
    {
        return Agents.SingleOrDefault(x => x.Id == id);
    }

    public IPvWayAgentPoolManagerAgent StartAgent<T>(
        string title,
        Action<T> repeat,
        T workerParam,
        TimeSpan sleepSpan,
        ThreadPriority priority = ThreadPriority.Normal,
        Action<IPvWayAgentPoolManagerAgent>? stopCallback = null)
    {
        var agent = new Agent<T>(
            OnAgentEnded,
            title,
            repeat,
            workerParam,
            sleepSpan,
            priority);
        _pool.Add(agent, stopCallback);
        return agent;
    }

    public IPvWayAgentPoolManagerAgent StartAgent(
        string title,
        Action repeat,
        TimeSpan sleepSpan,
        ThreadPriority priority = ThreadPriority.Normal,
        Action<IPvWayAgentPoolManagerAgent>? stopCallback = null)
    {
        var agent = new Agent(
            OnAgentEnded,
            title,
            repeat, 
            sleepSpan,
            priority);
        _pool.Add(agent, stopCallback);
        return agent;
    }

    private void OnAgentEnded(IPvWayAgentPoolManagerAgent agent)
    {
        _pool.TryGetValue(agent, out var stopCallback);
        stopCallback?.Invoke(agent);
        _pool.Remove(agent);
    }
}