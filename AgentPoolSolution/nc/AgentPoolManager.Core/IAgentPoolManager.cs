using System;
using System.Collections.Generic;
using System.Threading;

namespace pvWay.AgentPoolManager.Core
{
    public interface IAgentPoolManager
    {
        IEnumerable<IAgent> Agents { get; }

        IAgent GetAgent(Guid id);

        IAgent StartAgent<T>(
            string title,
            Action<T> repeat,
            T workerParam,
            TimeSpan sleepSpan,
            ThreadPriority priority = ThreadPriority.Normal,
            Action<IAgent> stopCallback = null);

        IAgent StartAgent(
            string title,
            Action repeat,
            TimeSpan sleepSpan,
            ThreadPriority priority = ThreadPriority.Normal,
            Action<IAgent> stopCallback = null);
    }
}