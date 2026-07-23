using System;
using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Events
{
    [CreateAssetMenu(fileName = "FloatGameEvent_", menuName = "Birthday Job Jam/Events/Float Game Event")]
    public sealed class FloatGameEvent : ScriptableObject
    {
        [Header("Editor Notes")]
        [SerializeField, TextArea] private string description;
        [SerializeField] private bool logWhenRaised;

        private readonly List<Action<float>> runtimeListeners = new List<Action<float>>();

        public event Action<float> Raised;

        public string Description => description;

        public void Raise(float value)
        {
            if (logWhenRaised)
                Debug.Log($"FloatGameEvent raised: {name} = {value}", this);

            Raised?.Invoke(value);

            for (int i = runtimeListeners.Count - 1; i >= 0; i--)
                runtimeListeners[i]?.Invoke(value);
        }

        public void RegisterRuntimeListener(Action<float> listener)
        {
            if (listener != null && !runtimeListeners.Contains(listener))
                runtimeListeners.Add(listener);
        }

        public void UnregisterRuntimeListener(Action<float> listener)
        {
            if (listener != null)
                runtimeListeners.Remove(listener);
        }
    }
}
