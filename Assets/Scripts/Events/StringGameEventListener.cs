using UnityEngine;
using UnityEngine.Events;

namespace BirthdayJobJam.Events
{
    public sealed class StringGameEventListener : MonoBehaviour
    {
        [SerializeField] private StringGameEvent gameEvent;
        [SerializeField] private UnityEvent<string> response;

        private void OnEnable()
        {
            if (gameEvent != null)
                gameEvent.Raised += OnEventRaised;
        }

        private void OnDisable()
        {
            if (gameEvent != null)
                gameEvent.Raised -= OnEventRaised;
        }

        private void OnEventRaised(string value)
        {
            response?.Invoke(value);
        }
    }
}
