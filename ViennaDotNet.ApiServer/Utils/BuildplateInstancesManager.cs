using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Net;
using Uma.Uuid;
using ViennaDotNet.Common.Utils;
using ViennaDotNet.EventBus.Client;

namespace ViennaDotNet.ApiServer.Utils
{
    public sealed class BuildplateInstancesManager
    {
        private readonly EventBusClient eventBusClient;
        private readonly Publisher publisher;
        private readonly Subscriber subscriber;

        private readonly Dictionary<string, TaskCompletionSource<bool>?> pendingInstances = new();
        private readonly Dictionary<string, InstanceInfo> instances = new();

        public BuildplateInstancesManager(EventBusClient eventBusClient)
        {
            this.eventBusClient = eventBusClient;
            this.publisher = eventBusClient.addPublisher();
            this.subscriber = eventBusClient.addSubscriber("buildplates", new Subscriber.SubscriberListener(
                _event => handleEvent(_event),
                () =>
                {
                    Log.Fatal("Buildplates event bus subscriber error");
                    Environment.Exit(1);
                }
            ));
        }

        public string? startBuildplateInstance(string playerId, string buildplateId, bool night)
        {
            string instanceId = U.RandomUuid().ToString();

            Log.Information($"Requesting buildplate instance {instanceId} for player {playerId} buildplate {buildplateId}");

            TaskCompletionSource<bool> completableFuture = new TaskCompletionSource<bool>();
            lock (pendingInstances)
                pendingInstances[instanceId] = completableFuture;

            if (!publisher.publish("buildplates", "startRequest", JsonConvert.SerializeObject(new StartRequest(instanceId, playerId, buildplateId, false, night))).Result)
            {
                Log.Error("Buildplates event bus publisher error");
                lock (pendingInstances)
                    pendingInstances.Remove(instanceId);

                return null;
            }

            if (!completableFuture.Task.Result)
            {
                Log.Warning($"Could not start buildplate instance {instanceId}");
                return null;
            }
            return instanceId;
        }

        public InstanceInfo? getInstanceInfo(string instanceId)
        {
            lock (instances)
                return instances.GetOrDefault(instanceId, null);
        }

        private void handleEvent(Subscriber.Event _event)

        {
            switch (_event.type)
            {
                case "started":
                    {
                        StartNotification startNotification;
                        try
                        {
                            startNotification = JsonConvert.DeserializeObject<StartNotification>(_event.data)!;

                            if (startNotification.info != null)
                            {
                                lock (instances)
                                {
                                    Log.Information($"Buildplate instance {startNotification.instanceId} has started");
                                    instances[startNotification.instanceId] = new InstanceInfo(
                                        startNotification.instanceId,
                                        startNotification.info.playerId,
                                        startNotification.info.buildplateId,
                                        startNotification.info.address,
                                        startNotification.info.port,
                                        false
                                    );
                                }
                            }

                            lock (pendingInstances)
                            {
                                TaskCompletionSource<bool>? completableFuture = pendingInstances[startNotification.instanceId];
                                pendingInstances.Remove(startNotification.instanceId);
                                if (completableFuture != null)
                                    completableFuture.SetResult(startNotification.info != null);
                            }
                        }
                        catch (Exception exception)
                        {
                            Log.Warning($"Bad start notification: {exception}");
                        }
                    }
                    break;
                case "ready":
                    {
                        string instanceId = _event.data;
                        lock (instances)
                        {
                            InstanceInfo? instanceInfo = instances.GetOrDefault(instanceId, null);
                            if (instanceInfo != null)
                            {
                                Log.Information($"Buildplate instance {instanceId} is ready");
                                instances[instanceId] = new InstanceInfo(
                                    instanceInfo.instanceId,
                                    instanceInfo.playerId,
                                    instanceInfo.buildplateId,
                                    instanceInfo.address,
                                    instanceInfo.port,
                                    true
                                );
                            }
                        }
                    }
                    break;
                case "stopped":
                    {
                        string instanceId = _event.data;
                        lock (instances)
                        {
                            if (instances.Remove(instanceId))
                                Log.Information($"Buildplate instance {instanceId} has stopped");
                        }
                    }
                    break;
            }
        }

        private record StartRequest(
            string instanceId,
            string playerId,
            string buildplateId,
            bool survival,
            bool night
        )
        {
        }

        private record StartNotification(
            string instanceId,
            StartNotification.Info? info
        )
        {
            public record Info(
                string playerId,
                string buildplateId,
                string address,
                int port
            )
            {
            }
        }

        public record InstanceInfo(
            string instanceId,

            string playerId,
            string buildplateId,

            string address,
            int port,

            bool ready
        )
        {
        }
    }
}
