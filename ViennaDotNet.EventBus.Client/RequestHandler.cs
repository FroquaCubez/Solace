namespace ViennaDotNet.EventBus.Client
{
    public sealed class RequestHandler
    {
        private readonly EventBusClient client;
        private readonly int channelId;
        private readonly string queueName;

        private readonly IHandler handler;

        private volatile bool closed = false;

        internal RequestHandler(EventBusClient client, int channelId, string queueName, IHandler handler)
        {
            this.client = client;
            this.channelId = channelId;
            this.queueName = queueName;
            this.handler = handler;
        }

        public void close()
        {
            closed = true;
            client.removeSubscriber(channelId);
            client.sendMessage(channelId, "CLOSE");
        }

        internal bool handleMessage(string message)
        {
            if (message == "ERR")
            {
                close();
                handler.error();
                return true;
            }
            else
            {
                string[] fields = message.Split(':', 4);
                if (fields.Length != 4)
                    return false;

                string requestIdString = fields[0];
                int requestId;
                try
                {
                    requestId = int.Parse(requestIdString);
                }
                catch (FormatException)
                {
                    return false;
                }

                if (requestId <= 0)
                    return false;

                string timestampString = fields[1];
                long timestamp;
                try
                {
                    timestamp = long.Parse(timestampString);
                }
                catch (FormatException)
                {
                    return false;
                }

                if (timestamp < 0)
                    return false;

                string type = fields[2];
                string data = fields[3];

                TaskCompletionSource<string?> responseCompletableFuture = handler.requestAsync(new Request(timestamp, type, data));
                responseCompletableFuture.Task.ContinueWith(task =>
                {
                    if (!closed)
                    {
                        if (task.Result != null)
                            client.sendMessage(channelId, "REP " + requestId + ":" + task.Result);
                        else
                            client.sendMessage(channelId, "NREP " + requestId);
                    }
                });

                return true;
            }
        }

        internal void error()
        {
            closed = true;
            handler.error();
        }

        public interface IHandler
        {
            TaskCompletionSource<string?> requestAsync(Request request)
            {
                TaskCompletionSource<string?> completableFuture = new();
                new Thread(() =>
                {
                    completableFuture.SetResult(this.request(request));
                }).Start();
                return completableFuture;
            }

            string? request(Request request);

            void error();
        }

        public class Handler : IHandler
        {
            public Func<Request, string?>? Request;
            public Action? Error;

            public Handler(Func<Request, string?>? _request, Action? _error)
            {
                Request = _request;
                Error = _error;
            }

            public string? request(Request request)
                => Request?.Invoke(request);

            public void error()
                => Error?.Invoke();
        }

        public sealed class Request
        {
            public readonly long timestamp;
            public readonly string type;
            public readonly string data;

            public Request(long timestamp, string type, string data)
            {
                this.timestamp = timestamp;
                this.type = type;
                this.data = data;
            }
        }
    }
}
