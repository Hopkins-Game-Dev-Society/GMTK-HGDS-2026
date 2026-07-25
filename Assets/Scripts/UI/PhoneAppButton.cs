using BirthdayJobJam.Core;
using BirthdayJobJam.UI;
using UnityEngine;

//To be attached to all of the app buttons

namespace BirthdayJobJam.Player
{
    public sealed class PhoneAppButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject appSelectionScreen;

        [SerializeField]
        private GameObject appScreen;

        public void AppOpen()
        {
            appSelectionScreen.SetActive(false);
            appScreen.SetActive(true);
        }

        //For out of date apps / just to fill the screen
        public void AppInvalid()
        {
            //IDK WHAT TO PUT HERE MAYBE A RANDOM TEXT BOX APPEARS FOR A SEC?
        }
       
    }
}