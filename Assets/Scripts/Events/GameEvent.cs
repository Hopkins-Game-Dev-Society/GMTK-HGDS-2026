using System;
using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Events
{
    [CreateAssetMenu(fileName = "GameEvent_", menuName = "Birthday Job Jam/Events/Game Event")]
    public sealed class GameEvent : ScriptableObject
    {
        [Header("Editor Notes")]
        [SerializeField, TextArea] private string description;
        [SerializeField] private bool logWhenRaised;

        private readonly List<GameEventListener> listeners = new List<GameEventListener>();
        private event Action RuntimeListeners;

        public string Description => description;

        public void Raise()
        {
            if (logWhenRaised)
                Debug.Log($"GameEvent raised: {name}", this);

            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                if (listeners[i] != null)
                    listeners[i].OnEventRaised();
            }

            RuntimeListeners?.Invoke();
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (listener != null && !listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (listener != null)
                listeners.Remove(listener);
        }

        public void RegisterRuntimeListener(Action callback)
        {
            RuntimeListeners += callback;
        }

        public void UnregisterRuntimeListener(Action callback)
        {
            RuntimeListeners -= callback;
        }
    }
}
