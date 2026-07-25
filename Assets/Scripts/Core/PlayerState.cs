using System;
using UnityEngine;

namespace BirthdayJobJam.Player
{
    public sealed class PlayerState : MonoBehaviour
    {
        [Header("Debug / Initial State")]
        [SerializeField]
        private bool phoneCollected;

        // Used to detect Inspector changes while playing.
        private bool previousPhoneCollected;

        public bool PhoneCollected => phoneCollected;

        public event Action PhoneCollectedChanged;

        private void Awake()
        {
            previousPhoneCollected = phoneCollected;
        }

        private void Update()
        {
            // If the value is changed in the Inspector during Play Mode,
            // notify listeners.
            if (phoneCollected != previousPhoneCollected)
            {
                previousPhoneCollected = phoneCollected;
                PhoneCollectedChanged?.Invoke();
            }
        }

        public void SetPhoneCollected(bool collected)
        {
            if (phoneCollected == collected)
                return;

            phoneCollected = collected;
            previousPhoneCollected = collected;

            PhoneCollectedChanged?.Invoke();
        }

        public void CollectPhone()
        {
            SetPhoneCollected(true);
        }

        public void RemovePhone()
        {
            SetPhoneCollected(false);
        }
    }
}

//Can use Game.Ctx.PlayerState.SetPhoneCollected(true); or collectPhone / removePhone

/*using System;
using UnityEngine;

namespace BirthdayJobJam.Player
{
    public sealed class PlayerState : MonoBehaviour
    {
        public bool PhoneCollected { get; private set; }

        public event Action PhoneCollectedChanged;

        public void CollectPhone()
        {
            if (PhoneCollected)
                return;

            PhoneCollected = true;
            PhoneCollectedChanged?.Invoke();
        }
    }
} */