using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/*
* This script should go on all text fields that need inputs from the virtural keyboard that exists within the game.
*/

namespace BirthdayJobJam.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public sealed class VirtualKeyboardTarget : MonoBehaviour, IPointerClickHandler
    {
        [Header("Selection Visual")]
        [SerializeField]
        private GameObject selectedIndicator;


        private TMP_InputField inputField;

        private bool isSelected;

        private float blinkTimer;

        private bool caretVisible;



        public TMP_InputField InputField => inputField;



        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();


            // Disable normal TMP editing behavior
            inputField.readOnly = true;


            // Disable TMP caret
            inputField.customCaretColor = true;
            inputField.caretColor = Color.clear;


            // Disable selection highlight
            inputField.selectionColor = Color.clear;


            // Disable native selection
            inputField.selectionAnchorPosition = 0;
            inputField.selectionFocusPosition = 0;


            if (selectedIndicator != null)
                selectedIndicator.SetActive(false);

        }



        private void Update()
        {
            if (!isSelected)
                return;
        }



        public void OnPointerClick(PointerEventData eventData)
        {
            VirtualKeyboardFocusManager.Instance?
                .ConsumeClick();


            Select();
        }



        public void Select()
        {
            isSelected = true;


            VirtualKeyboardController.Instance?
                .SetTarget(this);


            HideTMPSelection();


            if (selectedIndicator != null)
                selectedIndicator.SetActive(true);

        }



        public void Deselect()
        {
            isSelected = false;



            if (selectedIndicator != null)
                selectedIndicator.SetActive(false);


            HideTMPSelection();
        }



        public void Insert(string value)
        {
            int position =
                inputField.text.Length;


            inputField.SetTextWithoutNotify(
                inputField.text + value
            );


            HideTMPSelection();
        }



        public void Backspace()
        {
            if (inputField.text.Length == 0)
                return;


            string text =
                inputField.text.Substring(
                    0,
                    inputField.text.Length - 1
                );


            inputField.SetTextWithoutNotify(text);


            HideTMPSelection();
        }



        private void HideTMPSelection()
        {
            int position =
                inputField.text.Length;


            inputField.selectionAnchorPosition = position;
            inputField.selectionFocusPosition = position;
        }

    }
}