namespace pvWay.agentPoolManager.nc6
{
    public interface IAgent
    {
        Guid Id { get; }
        DateTime StartTimeUtc { get; }
        string Title { get; }
        void RequestToStop();
    }
}