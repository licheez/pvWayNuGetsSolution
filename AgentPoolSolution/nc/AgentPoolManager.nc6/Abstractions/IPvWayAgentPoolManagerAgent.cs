namespace pvWay.agentPoolManager.nc6.Abstractions;

public interface IPvWayAgentPoolManagerAgent
{
    Guid Id { get; }
    DateTime StartTimeUtc { get; }
    string Title { get; }
    void RequestToStop();
}
