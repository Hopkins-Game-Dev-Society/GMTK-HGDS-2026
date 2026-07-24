using BirthdayJobJam.Events;
using UnityEngine;
using UnityEngine.Events;

//Want to test my approach and see if it works where it just changes the currently active camera. It works

namespace BirthdayJobJam.Views
{
    public sealed class GameplayView : MonoBehaviour
    {
        [SerializeField] private GameViewId viewId;

        [Header("View Root")]
        [SerializeField] private GameObject root;

        [Header("Camera")]
        [SerializeField] private Camera viewCamera;

        [Header("Events")]
        [SerializeField] private GameEvent enteredEvent;
        [SerializeField] private GameEvent exitedEvent;

        [SerializeField] private UnityEvent entered;
        [SerializeField] private UnityEvent exited;


        public GameViewId ViewId => viewId;

        public Camera ViewCamera => viewCamera;

        public bool IsActive { get; private set; }

             private void Reset()
        {
            root = gameObject;
            viewCamera = GetComponentInChildren<Camera>();
        }


        private void Awake()
        {
            SetCameraActive(false);
        }


        public void Activate()
        {
            SetActive(true, true);
        }


        public void Deactivate()
        {
            SetActive(false, true);
        }


        public void ActivateSilently()
        {
            SetActive(true, false);
        }


        public void DeactivateSilently()
        {
            SetActive(false, false);
        }


        public void SetActive(bool active, bool sendEvents)
        {
            IsActive = active;

            SetCameraActive(active);


            if (!sendEvents)
                return;


            if (active)
            {
                entered?.Invoke();
                enteredEvent?.Raise();
            }
            else
            {
                exited?.Invoke();
                exitedEvent?.Raise();
            }
        }


        public void SetCameraActive(bool active)
        {
            if (viewCamera != null)
                viewCamera.enabled = active;
        }
    }
}