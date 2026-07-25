using UnityEngine;

namespace BirthdayJobJam.UI
{
    public sealed class VirtualKeyboardFocusManager : MonoBehaviour
    {
        public static VirtualKeyboardFocusManager Instance { get; private set; }


        private bool clickConsumed;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }


            Instance = this;
        }



        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;


            if (clickConsumed)
            {
                clickConsumed = false;
                return;
            }


            VirtualKeyboardController.Instance?
                .ClearTarget();
        }



        public void ConsumeClick()
        {
            clickConsumed = true;
        }
    }
}
