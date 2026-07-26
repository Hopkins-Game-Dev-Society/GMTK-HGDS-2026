using BirthdayJobJam.Core;
using UnityEngine;

namespace BirthdayJobJam.Player
{
    public sealed class PhonePickup : MonoBehaviour
    {
        [SerializeField]
        private bool disableAfterPickup = true;


        private void OnMouseDown()
        {
            CollectPhone();
        }


        private void CollectPhone()
        {
            if (Game.Ctx == null)
            {
                Debug.LogWarning("PhonePickup: No GameContext found.");
                return;
            }


            if (Game.Ctx.PlayerState == null)
            {
                Debug.LogWarning("PhonePickup: No PlayerState found.");
                return;
            }


            Game.Ctx.PlayerState.SetPhoneCollected(true);


            if (disableAfterPickup)
            {
                gameObject.SetActive(false);
            }
        }
    }
}