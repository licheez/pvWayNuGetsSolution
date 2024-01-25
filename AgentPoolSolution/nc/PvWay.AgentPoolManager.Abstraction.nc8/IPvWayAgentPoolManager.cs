namespace PvWay.AgentPoolManager.Abstraction.nc8;

public interface IPvWayAgentPoolManager
{
    IEnumerable<IPvWayAgentPoolManagerAgent> Agents { get; }

    IPvWayAgentPoolManagerAgent? GetAgent(Guid id);

    IPvWayAgentPoolManagerAgent StartAgent<T>(
        string title,
        Action<T> repeat,
        T workerParam,
        TimeSpan sleepSpan,
        ThreadPriority priority = ThreadPriority.Normal,
        Action<IPvWayAgentPoolManagerAgent>? stopCallback = null);

    IPvWayAgentPoolManagerAgent StartAgent(
        string title,
        Action repeat,
        TimeSpan sleepSpan,
        ThreadPriority priority = ThreadPriority.Normal,
        Action<IPvWayAgentPoolManagerAgent>? stopCallback = null);
}