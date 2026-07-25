using System;
using UnityEngine;

namespace BirthdayJobJam.Player
{
    public sealed class PlayerState : MonoBehaviour
    {
        [Header("Debug / Initial State")]
        [SerializeField]
        private bool phoneCollected;

        private bool phoneActive;

        // Used to detect Inspector changes while playing.
        private bool previousPhoneCollected;

        public bool PhoneCollected => phoneCollected;

        public event Action PhoneCollectedChanged;

        //Need to add a phone active trigger here as well.

        public bool PhoneActive => phoneActive;

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

        //Just reverses the current indicator
        public void SetPhoneOnOff()
        {
            if (phoneActive == true)
            {
                phoneActive = false;
            } else
            {
                phoneActive = true;
            }
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