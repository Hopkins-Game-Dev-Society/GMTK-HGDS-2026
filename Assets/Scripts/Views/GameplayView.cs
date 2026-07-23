using BirthdayJobJam.Events;
using UnityEngine;
using UnityEngine.Events;

//Want to test my approach and see if it works where it just changes the currently active camera. 


/*
namespace BirthdayJobJam.Views
{
    public sealed class GameplayView : MonoBehaviour
    {
        [SerializeField] private GameViewId viewId;

        [Header("View Root")]
        [SerializeField] private GameObject root;


        [Header("Events")]
        [SerializeField] private GameEvent enteredEvent;
        [SerializeField] private GameEvent exitedEvent;
        [SerializeField] private UnityEvent entered;
        [SerializeField] private UnityEvent exited;
        
        public GameViewId ViewId => viewId;
        public bool IsActive { get; private set; }

        [System.Serializable]
        public class NavigationLink
        {
            public ViewDirection direction;
            public GameViewId destination;
        }

        [SerializeField]
        private List<NavigationLink> navigation = new();


        //So when this is active it makes this camera activated, and disables all other cameras. 

        private void Reset()
        {
            root = gameObject;
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

       private void SetActive(bool active, bool sendEvents)
        {
            IsActive = active;

            if (root != null && root.activeSelf != active)
                root.SetActive(active);

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
        public bool TryGetDestination(ViewDirection direction, out GameViewId destination)
        {
            foreach (NavigationLink link in navigation)
            {
                if (link.direction == direction)
                {
                    destination = link.destination;
                    return true;
                }
            }

            destination = GameViewId.None;
            return false;
        }
    }
}
*/
namespace BirthdayJobJam.Views
{
    public sealed class GameplayView : MonoBehaviour
    {
        [SerializeField] private GameViewId viewId;

        [Header("View Root")]
        [SerializeField] private GameObject root;

        [Header("Events")]
        [SerializeField] private GameEvent enteredEvent;
        [SerializeField] private GameEvent exitedEvent;

        [SerializeField] private UnityEvent entered;
        [SerializeField] private UnityEvent exited;


        public GameViewId ViewId => viewId;

        public bool IsActive { get; private set; }


        private void Awake()
        {
            if (root == null)
                root = gameObject;
        }


        private void Reset()
        {
            root = gameObject;
        }


        public void Activate()
        {
            SetViewActive(true, true);
        }


        public void Deactivate()
        {
            SetViewActive(false, true);
        }


        public void ActivateSilently()
        {
            SetViewActive(true, false);
        }


        public void DeactivateSilently()
        {
            SetViewActive(false, false);
        }


        private void SetViewActive(bool active, bool sendEvents)
        {
            IsActive = active;


            if (root != null && root.activeSelf != active)
                root.SetActive(active);


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
    }
}