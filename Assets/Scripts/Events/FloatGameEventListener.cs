using UnityEngine;
using UnityEngine.Events;

namespace BirthdayJobJam.Events
{
    public sealed class FloatGameEventListener : MonoBehaviour
    {
        [SerializeField] private FloatGameEvent gameEvent;
        [SerializeField] private UnityEvent<float> response;

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

        private void OnEventRaised(float value)
        {
            response?.Invoke(value);
        }
    }
}
