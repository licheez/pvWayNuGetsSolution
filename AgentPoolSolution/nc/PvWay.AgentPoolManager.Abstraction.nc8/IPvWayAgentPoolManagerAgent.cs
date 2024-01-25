namespace PvWay.AgentPoolManager.Abstraction.nc8;

public interface IPvWayAgentPoolManagerAgent
{
    Guid Id { get; }
    DateTime StartTimeUtc { get; }
    string Title { get; }
    void RequestToStop();
}
