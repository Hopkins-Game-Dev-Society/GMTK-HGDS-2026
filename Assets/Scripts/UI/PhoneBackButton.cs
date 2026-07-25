using System.Collections.Generic;
using BirthdayJobJam.Core;
using BirthdayJobJam.UI;
using UnityEngine;

//To be attached to the back buttonh

namespace BirthdayJobJam.Player
{
    public sealed class PhoneBackButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject appSelectionScreen;

        [SerializeField]
        private List<GameObject> appScreens = new();

        public void HomeReturn()
        {
            appSelectionScreen.SetActive(true);
            foreach (GameObject screen in appScreens)
            {
                screen.SetActive(false);
            }

        }
       
    }
}