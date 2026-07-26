using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Player
{
    public sealed class PhoneBackButtonC : MonoBehaviour
    {
        [SerializeField]
        private GameObject appSelectionScreen;

        [SerializeField]
        private PhoneEventManager phoneEvent;



        [SerializeField]
        private List<GameObject> appScreens = new();


        public void HomeReturn()
        {
            PhoneRingtoneController.Instance?
                .StopRinging();

            phoneEvent?.EndPhoneMessage();


            if (appSelectionScreen != null)
                appSelectionScreen.SetActive(true);


            foreach (GameObject screen in appScreens)
            {
                if (screen != null)
                    screen.SetActive(false);
            }
        }
    }
}