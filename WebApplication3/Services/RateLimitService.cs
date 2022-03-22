using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace WebApplication3
{
    public interface IRateLimitService
    {
        public bool IsRateLimitReached(StringValues clientQuery, int threshold);
    }

    public sealed class RateLimitService : IRateLimitService
    {
        private readonly object _lock = new object();

        private Dictionary<string, ClientRateLimit> ClientRequestsData { get; } =
            new Dictionary<string, ClientRateLimit>();
        private TimeSpan TimeFrame { get; } = new TimeSpan(0, 0, 0, 5);

        public bool IsRateLimitReached(StringValues clientQuery, int threshold)
        {
            lock (_lock)
            {
                if (!ClientRequestsData.TryGetValue(clientQuery, out var clientRateLimit))
                {
                    ClientRequestsData[clientQuery] = new ClientRateLimit(1, DateTime.Now);
                    return false;
                }
                else
                {
                    if (clientRateLimit.IsTimeInRange(DateTime.Now, TimeFrame))

                    {
                        if (!clientRateLimit.IsExceededLimit(threshold))
                        {
                            clientRateLimit.IncrementRequestCount();
                            return false;
                        }
                    }
                    else
                    {
                        clientRateLimit.Reset(1, DateTime.Now);
                        return false;
                    }
                }
                return true;
            }
        }

        private class ClientRateLimit
        {
            private int _requestCount;
            private DateTime _timeOfFirstRequest;

            public ClientRateLimit(int requestCount, DateTime timeOfFirstRequest)
            {
                _requestCount = requestCount;
                _timeOfFirstRequest = timeOfFirstRequest;
            }

            public void IncrementRequestCount()
            {
                _requestCount++;
            }

            public bool IsExceededLimit(int number)
            {
                return _requestCount > number;
            }

            public void Reset(int counter, DateTime timeOfFirstRequest)
            {
                _requestCount = counter;
                _timeOfFirstRequest = timeOfFirstRequest;
            }

            public bool IsTimeInRange(DateTime time, TimeSpan span)
            {
                return (time - _timeOfFirstRequest) < span;
            }
        }
    }
}