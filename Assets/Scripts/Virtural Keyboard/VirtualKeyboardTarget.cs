using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/*
* This script should go on all text fields that need inputs from the virtural keyboard that exists within the game.
*/

/*
namespace BirthdayJobJam.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public sealed class VirtualKeyboardTarget : MonoBehaviour
    {
        private TMP_InputField inputField;

        public TMP_InputField InputField => inputField;

        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();

            // Prevent hardware keyboard typing
            inputField.readOnly = true;
        }

        public void BeginEditing()
        {
            VirtualKeyboardController.Instance?.SetTarget(this);

            inputField.ActivateInputField();
            inputField.Select();
        }

        public void EndEditing()
        {
            inputField.DeactivateInputField();
        }

        public void Insert(string text)
        {
            int caret = inputField.stringPosition;

            inputField.text = inputField.text.Insert(caret, text);

            caret += text.Length;

            inputField.stringPosition = caret;
            inputField.caretPosition = caret;

            inputField.ForceLabelUpdate();
        }

        public void Backspace()
        {
            int caret = inputField.stringPosition;

            if (caret == 0)
                return;

            inputField.text = inputField.text.Remove(caret - 1, 1);

            caret--;

            inputField.stringPosition = caret;
            inputField.caretPosition = caret;

            inputField.ForceLabelUpdate();
        }
    }
} */

/*
namespace BirthdayJobJam.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public sealed class VirtualKeyboardTarget :
        MonoBehaviour,
        IPointerClickHandler
    {
        private TMP_InputField inputField;

        public TMP_InputField InputField => inputField;

        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();

            // Prevent physical keyboard input.
            inputField.readOnly = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            BeginEditing();
        }

        public void BeginEditing()
        {
            VirtualKeyboardController.Instance?.SetTarget(this);

            inputField.ActivateInputField();
            inputField.Select();
        }


        public void EndEditing()
        {
            inputField.DeactivateInputField();
        }

        public void Insert(string text)
        {
            int caret = inputField.stringPosition;

            inputField.text = inputField.text.Insert(caret, text);

            caret += text.Length;

            inputField.stringPosition = caret;
            inputField.caretPosition = caret;

            inputField.ForceLabelUpdate();
        }

        public void Backspace()
        {
            int caret = inputField.stringPosition;

            if (caret == 0)
                return;

            inputField.text = inputField.text.Remove(caret - 1, 1);

            caret--;

            inputField.stringPosition = caret;
            inputField.caretPosition = caret;

            inputField.ForceLabelUpdate();
        }
    }
} */

/*
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BirthdayJobJam.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public sealed class VirtualKeyboardTarget :
        MonoBehaviour,
        IPointerClickHandler
    {
        [Header("Selection Visual")]
        [SerializeField]
        private GameObject selectedIndicator;


        private TMP_InputField inputField;


        public TMP_InputField InputField => inputField;



        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();

            inputField.readOnly = true;


            if (selectedIndicator != null)
                selectedIndicator.SetActive(false);
        }

        private void OnDisable()
{
    Debug.Log("VirtualKeyboardTarget disabled");
}



        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
        }


        public void Select()
{
    VirtualKeyboardFocusManager.Instance?
        .ConsumeClick();


    VirtualKeyboardController.Instance?
        .SetTarget(this);


    if (selectedIndicator != null)
        selectedIndicator.SetActive(true);
}



        public void Deselect()
        {
            inputField.DeactivateInputField();


            if (selectedIndicator != null)
                selectedIndicator.SetActive(false);
        }



        public void Insert(string text)
        {
            int caret = inputField.stringPosition;


            inputField.text =
                inputField.text.Insert(caret, text);


            caret += text.Length;


            inputField.stringPosition = caret;
            inputField.caretPosition = caret;


            inputField.ForceLabelUpdate();
        }



        public void Backspace()
        {
            int caret = inputField.stringPosition;


            if (caret == 0)
                return;


            inputField.text =
                inputField.text.Remove(caret - 1, 1);


            caret--;


            inputField.stringPosition = caret;
            inputField.caretPosition = caret;


            inputField.ForceLabelUpdate();
        }
    }
} */

namespace BirthdayJobJam.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public sealed class VirtualKeyboardTarget : MonoBehaviour, IPointerClickHandler
    {
        [Header("Selection Visual")]
        [SerializeField]
        private GameObject selectedIndicator;


        private TMP_InputField inputField;


        public TMP_InputField InputField => inputField;



        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();

            // Prevent physical keyboard input
            inputField.readOnly = true;


            if (selectedIndicator != null)
                selectedIndicator.SetActive(false);
        }



        public void OnPointerClick(PointerEventData eventData)
        {
            // This click was handled.
            // The focus manager should not clear focus.
            VirtualKeyboardFocusManager.Instance?
                .ConsumeClick();


            Select();
        }



        public void Select()
        {
            VirtualKeyboardController.Instance?
                .SetTarget(this);


            inputField.caretPosition = inputField.text.Length;
            inputField.stringPosition = inputField.text.Length;


            if (selectedIndicator != null)
                selectedIndicator.SetActive(true);
        }



        public void Deselect()
        {
            if (selectedIndicator != null)
                selectedIndicator.SetActive(false);
        }



        public void Insert(string value)
        {
            int caretPosition = inputField.caretPosition;


            string newText =
                inputField.text.Insert(
                    caretPosition,
                    value
                );


            inputField.SetTextWithoutNotify(newText);


            caretPosition += value.Length;


            inputField.caretPosition = caretPosition;
            inputField.stringPosition = caretPosition;


            inputField.ForceLabelUpdate();
        }



        public void Backspace()
        {
            int caretPosition = inputField.caretPosition;


            if (caretPosition <= 0)
                return;


            string newText =
                inputField.text.Remove(
                    caretPosition - 1,
                    1
                );


            inputField.SetTextWithoutNotify(newText);


            caretPosition--;


            inputField.caretPosition = caretPosition;
            inputField.stringPosition = caretPosition;


            inputField.ForceLabelUpdate();
        }
    }
}