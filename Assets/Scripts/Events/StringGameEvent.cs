using System;
using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Events
{
    [CreateAssetMenu(fileName = "StringGameEvent_", menuName = "Birthday Job Jam/Events/String Game Event")]
    public sealed class StringGameEvent : ScriptableObject
    {
        [Header("Editor Notes")]
        [SerializeField, TextArea] private string description;
        [SerializeField] private bool logWhenRaised;

        private readonly List<Action<string>> runtimeListeners = new List<Action<string>>();

        public event Action<string> Raised;

        public string Description => description;

        public void Raise(string value)
        {
            if (logWhenRaised)
                Debug.Log($"StringGameEvent raised: {name} = {value}", this);

            Raised?.Invoke(value);

            for (int i = runtimeListeners.Count - 1; i >= 0; i--)
                runtimeListeners[i]?.Invoke(value);
        }

        public void RegisterRuntimeListener(Action<string> listener)
        {
            if (listener != null && !runtimeListeners.Contains(listener))
                runtimeListeners.Add(listener);
        }

        public void UnregisterRuntimeListener(Action<string> listener)
        {
            if (listener != null)
                runtimeListeners.Remove(listener);
        }
    }
}
