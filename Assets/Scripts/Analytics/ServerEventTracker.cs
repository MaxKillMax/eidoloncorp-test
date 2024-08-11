using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Test.Parsers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

namespace Test.ServerEvents
{
    public class ServerEventTracker
    {
        private const string SERVER_URL = "";
        private const string SAVE_KEY = "ServerEventTrackerEvents";

        private const int COOLDOWN_BEFORE_SEND = 3;
        private const int REQUEST_WAITING_TIME = 10;

        private static readonly List<ServerEvent> TrackedEvents = new();
        private static bool IsSending = false;

        static ServerEventTracker()
        {
            if (!PlayerPrefs.HasKey(SAVE_KEY))
                return;

            string content = PlayerPrefs.GetString(SAVE_KEY);
            TrackedEvents = Parser.Deserialize<List<ServerEvent>>(content);

            if (TrackedEvents.Count != 0)
                TrySendAll();
        }

        /// <summary>
        /// Requires for static constructor
        /// </summary>
        public static void Start() { }

        public static void TrackEvent(string type, string data) => TrackEvent(new ServerEvent(type, data));

        public static void TrackEvent(ServerEvent @event)
        {
            TrackedEvents.Add(@event);
            SaveEvents();

            TrySendAll();
        }

        private static async void TrySendAll()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable || IsSending)
                return;

            IsSending = true;
            await WaitWhileAsync(() => true, COOLDOWN_BEFORE_SEND);

            ServerEventCluster cluster = new(TrackedEvents);
            string content = Parser.Serialize(cluster);
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            if (await Request(bytes))
            {
                TrackedEvents.RemoveAll(s => cluster.Events.Contains(s));
                SaveEvents();
            }

            IsSending = false;

            static async UniTask WaitWhileAsync(Func<bool> condition, int time)
            {
                float remainTime = time;

                while (condition.Invoke() && remainTime > 0)
                {
                    await UniTask.DelayFrame(1);
                    remainTime -= Time.deltaTime;
                }
            }

            static async UniTask<bool> Request(byte[] bytes)
            {
                using UnityWebRequest request = new(SERVER_URL, "POST");
                request.uploadHandler = new UploadHandlerRaw(bytes);

                UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                await WaitWhileAsync(() => !operation.isDone, REQUEST_WAITING_TIME);

                if (!string.IsNullOrEmpty(operation.webRequest.error))
                    Debug.LogWarning($"Server event sending error: {operation.webRequest.error}");

                return operation.webRequest.responseCode == 200;
            }
        }

        private static void SaveEvents()
        {
            string content = Parser.Serialize(TrackedEvents);
            PlayerPrefs.SetString(SAVE_KEY, content);
        }
    }
}
