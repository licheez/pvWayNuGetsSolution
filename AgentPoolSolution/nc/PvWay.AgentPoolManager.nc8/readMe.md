# Agent Pool Manager Core

Manages a pool of background services (agents) that keep repeating at a given interval until they are requested to stop.

This lets your Web application runs a pool of tasks in the background.

## Interfaces

This nuGet has only one public class implementing the following interface

### IAgentPoolManager

```csharp
namespace pvWay.agentPoolManager.nc6;

public interface IAgentPoolManager
{
    IEnumerable<IAgent> Agents { get; }

    IAgent? GetAgent(Guid id);

    IAgent StartAgent<T>(
        string title,
        Action<T> repeat,
        T workerParam,
        TimeSpan sleepSpan,
        ThreadPriority priority = ThreadPriority.Normal,
        Action<IAgent>? stopCallback = null);
    
    IAgent StartAgent(
        string title,
        Action repeat,
        TimeSpan sleepSpan,
        ThreadPriority priority = ThreadPriority.Normal,
        Action<IAgent>? stopCallback = null);
    
}
```

### IAgent

```csharp
namespace pvWay.agentPoolManager.nc6;

public interface IAgent
{
    Guid Id { get; }
    DateTime StartTimeUtc { get; }
    string Title { get; }
    void RequestToStop();
}
```

## Usage

See here after a short Console that use the pool

### Principe

* Create a method (with or without parameter) that you want to repeatedly invoke in background
* Determine the interval of time between two invocations of your method
* Instantiate the PoolManager (you can wrap/inject this class into/as a Singleton)
* Add your method into the Agent Pool and in return get a IAgent reference
* Stop the method at any time by calling the IAgent RequestToStop method

The following example shows the code for a simple clock pulsar that write the time in the console every 5 seconds.

### The code

```csharp
using pvWay.agentPoolManager.nc6;

Console.WriteLine("Hello, AgentPool");

var apm = new PoolManager();

var pulsar = apm.StartAgent(
    // the name of the asynchronous agent
    "pulsar",
    // the method to repeat asynchronously
    Pulsar,
    // the string param passed to the Pulsar method
    "clock",
    // time between each invocation
    TimeSpan.FromSeconds(5),
    // the priority
    ThreadPriority.Normal,
    // the lambda that is called when the pulsar is stopped
    agent => Console.WriteLine($"{agent.Title} is stopped"));

Console.WriteLine("hit a key to stop");
Console.ReadKey();

pulsar.RequestToStop();

return;

static void Pulsar(string prefix)
{
    Console.WriteLine($"{prefix}-{DateTime.Now:HH:mm:ss}");
}

```

Happy coding