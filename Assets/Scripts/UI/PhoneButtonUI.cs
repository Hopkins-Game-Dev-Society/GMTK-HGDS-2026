using BirthdayJobJam.Core;
using UnityEngine;

//Should be attached to the button and makes it visible only if the phone has been collected

namespace BirthdayJobJam.Player
{
    public sealed class PhoneButtonUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject button;

        private PlayerState playerState;

        private void Start()
        {
            if (button == null)
            {
                Debug.LogWarning("PhoneButtonUI: Button reference is missing.", this);
                return;
            }

            playerState = Game.Ctx?.PlayerState;

            if (playerState == null)
            {
                Debug.LogWarning("PhoneButtonUI: PlayerState not found.", this);
                button.SetActive(false);
                return;
            }

            Refresh();

            playerState.PhoneCollectedChanged += Refresh;
        }

        private void OnDestroy()
        {
            if (playerState != null)
                playerState.PhoneCollectedChanged -= Refresh;
        }

        private void Refresh()
        {
            button.SetActive(playerState.PhoneCollected);
        }
    }
}