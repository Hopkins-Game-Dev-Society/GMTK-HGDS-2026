using UnityEngine;

namespace BirthdayJobJam.Interaction
{
    public sealed class InspectableObject : MonoBehaviour
    {
        [Header("Inspection UI")]
        [SerializeField]
        private GameObject inspectionScreen;

        [SerializeField]
        private GameObject imageToDisplay;

        [SerializeField]
        private GameObject imageBackground;


        private void Start()
        {
            if (inspectionScreen != null)
                inspectionScreen.SetActive(false);

            if (imageToDisplay != null)
                imageToDisplay.SetActive(false);
        }


        private void OnMouseDown()
        {
            OpenInspection();
        }


        public void OpenInspection()
        {
            if (inspectionScreen == null)
                return;


            inspectionScreen.SetActive(true);


            if (imageToDisplay != null)
                imageToDisplay.SetActive(true);

            if (imageBackground != null)
                imageBackground.SetActive(true);
        }


        public void CloseInspection()
        {
            if (inspectionScreen != null)
                inspectionScreen.SetActive(false);


            if (imageToDisplay != null)
                imageToDisplay.SetActive(false);
        }
    }
}