using UnityEngine;
using UnityEngine.EventSystems;

namespace BirthdayJobJam.Interaction
{
    public sealed class InspectionCloseButton :
        MonoBehaviour,
        IPointerClickHandler
    {
        [SerializeField]
        private GameObject inspectionScreen;

        [SerializeField]
        private GameObject closeItemID;


        public void OnPointerClick(PointerEventData eventData)
        {
            if (inspectionScreen != null)
                inspectionScreen.SetActive(false);
            
            if (closeItemID != null)
                closeItemID.SetActive(false);
        }
    }
}