namespace pvWay.agentPoolManager.nc6
{
    public class PoolManager : IAgentPoolManager
    {
        private readonly IDictionary<IAgent, Action<IAgent>?> _pool = 
            new Dictionary<IAgent, Action<IAgent>?>();
        public IEnumerable<IAgent> Agents => _pool.Keys;

        public IAgent? GetAgent(Guid id)
        {
            return Agents.SingleOrDefault(x => x.Id == id);
        }

        public IAgent StartAgent<T>(
            string title,
            Action<T> repeat,
            T workerParam,
            TimeSpan sleepSpan,
            ThreadPriority priority = ThreadPriority.Normal,
            Action<IAgent>? stopCallback = null)
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

        public IAgent StartAgent(
            string title,
            Action repeat,
            TimeSpan sleepSpan,
            ThreadPriority priority = ThreadPriority.Normal,
            Action<IAgent>? stopCallback = null)
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

        private void OnAgentEnded(IAgent agent)
        {
            _pool.TryGetValue(agent, out var stopCallback);
            stopCallback?.Invoke(agent);
            _pool.Remove(agent);
        }
    }
}
