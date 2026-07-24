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


/*
using UnityEngine;
using UnityEngine.EventSystems;

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


            // Something explicitly handled this click
            if (clickConsumed)
            {
                clickConsumed = false;
                return;
            }


            // Clicking normal UI should not clear focus
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }


            // Nothing claimed this click
            VirtualKeyboardController.Instance?
                .ClearTarget();
        }


        public void ConsumeClick()
        {
            clickConsumed = true;
        }
    }
} */


/*
using UnityEngine;
using UnityEngine.EventSystems;
using BirthdayJobJam.Core;

namespace BirthdayJobJam.UI
{
    public sealed class VirtualKeyboardFocusManager : MonoBehaviour
    {
        //[SerializeField]
        //private Camera activeCamera;

        private Camera ActiveCamera =>
            Game.Ctx?.Views?.CurrentCamera;


        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;


            // Canvas UI click?
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Camera camera = ActiveCamera;
            // World click?
            if (camera != null)
            {
                Vector2 worldPosition =
                    camera.ScreenToWorldPoint(
                        Input.mousePosition);


                RaycastHit2D hit =
                    Physics2D.Raycast(
                        worldPosition,
                        Vector2.zero);

                if (hit.collider != null)
                {
                    Debug.Log($"Clicked: {hit.collider.name}");

                    if (hit.collider.GetComponentInParent<VirtualKeyboardKey>() != null)
                    {
                        Debug.Log("Keyboard key detected");
                        return;
                    }
                }
            }


            // Everything else clears focus
            VirtualKeyboardController.Instance?.ClearTarget();
        }
    }
} */

/*
using UnityEngine;
using UnityEngine.EventSystems;

namespace BirthdayJobJam.UI
{
    public sealed class VirtualKeyboardFocusManager : MonoBehaviour
    {
        [SerializeField]
        private Camera activeCamera;


        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;


            // If a Canvas UI element was clicked,
            // do not clear the input focus.
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }


            // Check world objects (keyboard keys, interactables, etc.)
            if (activeCamera != null)
            {
                Vector2 worldPosition =
                    activeCamera.ScreenToWorldPoint(
                        Input.mousePosition);


                RaycastHit2D hit =
                    Physics2D.Raycast(
                        worldPosition,
                        Vector2.zero);


                if (hit.collider != null)
                {
                    // Clicking a virtual keyboard key should preserve focus
                    if (hit.collider.GetComponent<VirtualKeyboardKey>() != null)
                    {
                        return;
                    }
                }
            }


            // If we reach here:
            // - not UI
            // - not keyboard
            // clear input focus
            VirtualKeyboardController.Instance?.ClearTarget();
        }
    }
} */

/*using UnityEngine;
using UnityEngine.EventSystems;

namespace BirthdayJobJam.UI
{
    public sealed class VirtualKeyboardFocusManager : MonoBehaviour
    {
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;


            // Ignore UI clicks
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Camera cam = Camera.main;

            if (cam == null)
                return;

            Vector2 worldPosition =
                cam.ScreenToWorldPoint(Input.mousePosition);


            RaycastHit2D hit =
                Physics2D.Raycast(worldPosition,Vector2.zero);


            // Something in the world was clicked
            if (hit.collider != null)
            {
                // Input field was clicked
                if (hit.collider.GetComponent<VirtualKeyboardTarget>() != null)
                    return;


                // Keyboard key was clicked
                if (hit.collider.GetComponent<VirtualKeyboardKey>() != null)
                    return;
            }


            // Nothing important was clicked
            VirtualKeyboardController.Instance?.ClearTarget();
        }
    }
} */

/* using UnityEngine;
using UnityEngine.EventSystems;

namespace BirthdayJobJam.UI
{
    public sealed class VirtualKeyboardFocusManager : MonoBehaviour
    {
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;


            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }


            VirtualKeyboardController.Instance?
                .ClearTarget();
        }
    }
} */