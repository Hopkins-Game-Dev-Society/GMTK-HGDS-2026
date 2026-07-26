using UnityEngine;

namespace BirthdayJobJam.Player
{
    public sealed class PhoneCallButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject phoneScreen;


        public void AnswerCall()
        {
            PhoneRingtoneController.Instance?
                .StopRinging();


            if (phoneScreen != null)
            {
                phoneScreen.SetActive(true);
            }
        }


        public void ReceiveCall()
        {
            PhoneRingtoneController.Instance?
                .StartRinging();
        }
    }
}