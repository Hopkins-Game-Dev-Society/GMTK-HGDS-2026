using UnityEngine;

/*
* Attached to each individual key, can be modified with the concept for having different keys like a specific "special character key"
*/

namespace BirthdayJobJam.UI
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class VirtualKeyboardKey : MonoBehaviour
    {
        public enum KeyType
        {
            Character,
            Space,
            Backspace,
            Shift,
            CapsLock,
            Enter
        }

        [SerializeField]
        private KeyType keyType = KeyType.Character;

        [SerializeField]
        private string value = "";

        private void OnMouseDown()
        {
            VirtualKeyboardFocusManager.Instance?.ConsumeClick();

            Press();
        }

        public void Press()
        {
            VirtualKeyboardController controller = VirtualKeyboardController.Instance;

            if (controller == null)
                return;

            switch (keyType)
            {
                case KeyType.Character:
                    controller.InsertCharacter(value);
                    break;

                case KeyType.Space:
                    controller.InsertSpace();
                    break;

                case KeyType.Backspace:
                    controller.Backspace();
                    break;

                case KeyType.Shift:
                    controller.ToggleShift();
                    break;

                case KeyType.CapsLock:
                    controller.ToggleCapsLock();
                    break;

                case KeyType.Enter:
                    controller.Finish();
                    break;
            }
        }
    }
}