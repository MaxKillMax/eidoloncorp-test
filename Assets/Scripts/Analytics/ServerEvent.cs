using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Test.ServerEvents
{
    [Serializable]
    public class ServerEvent
    {
        [JsonProperty("type"), SerializeField] private string _type;
        [JsonProperty("data"), SerializeField] private string _data;

        public ServerEvent(string type, string data)
        {
            _type = type;
            _data = data;
        }
    }
}
