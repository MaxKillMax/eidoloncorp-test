using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Test.ServerEvents
{
    /// <summary>
    /// Required for request format like in test task
    /// </summary>
    [Serializable]
    public class ServerEventCluster
    {
        [JsonProperty("type"), SerializeField] private ServerEvent[] _events;
        public ServerEvent[] Events => _events;

        public ServerEventCluster(IEnumerable<ServerEvent> events)
        {
            _events = events.ToArray();
        }
    }
}
