﻿using System;
using System.Threading;
using pvWay.AgentPoolManager;

namespace AgentPoolManagerLab
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
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

            Console.WriteLine("hit a key to quit");
            Console.ReadKey();
        }

        private static void Pulsar(string prefix)
        {
            Console.WriteLine($"{prefix}-{DateTime.Now:HH:mm:ss}");
        }

    }
}
