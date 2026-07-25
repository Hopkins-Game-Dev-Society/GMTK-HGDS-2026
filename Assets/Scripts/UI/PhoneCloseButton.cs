using BirthdayJobJam.Core;
using BirthdayJobJam.UI;
using UnityEngine;

//To be attached to the power button

namespace BirthdayJobJam.Player
{
    public sealed class PhoneCloseButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject PhoneUIbutton;

        [SerializeField]
        private UISlidePanel phone;

        public void PhoneClose()
        {
            phone.Hide();
            PhoneUIbutton.SetActive(true);

        }
       
    }
}