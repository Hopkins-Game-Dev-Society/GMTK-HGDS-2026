using UnityEngine;
using UnityEngine.EventSystems;

//This should be attached to UI buttons 

namespace BirthdayJobJam.UI
{
    public sealed class CursorHoverUI :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {

        public void OnPointerEnter(PointerEventData eventData)
        {
            HandCursorController.Instance?
                .SetPointing(true);
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            HandCursorController.Instance?
                .SetPointing(false);
        }
    }
}