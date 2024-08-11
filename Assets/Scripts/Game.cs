using NaughtyAttributes;
using Test.ServerEvents;
using UnityEngine;

namespace Test
{
    public class Game : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            ServerEventTracker.Start();
        }

#if UNITY_EDITOR
        [SerializeField] private ServerEvent _testEvent;

        [Button(nameof(SendEventTest), EButtonEnableMode.Playmode)]
        private void SendEventTest() => ServerEventTracker.TrackEvent(_testEvent);
#endif
    }
}
