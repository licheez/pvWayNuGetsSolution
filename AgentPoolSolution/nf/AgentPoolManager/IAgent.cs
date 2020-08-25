using System;

namespace pvWay.AgentPoolManager
{
    public interface IAgent
    {
        Guid Id { get; }
        DateTime StartTimeUtc { get; }
        string Title { get; }
        void RequestToStop();
    }
}