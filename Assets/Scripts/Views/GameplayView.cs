using BirthdayJobJam.Events;
using UnityEngine;
using UnityEngine.Events;

namespace BirthdayJobJam.Views
{
    public sealed class GameplayView : MonoBehaviour
    {
        [SerializeField] private GameViewId viewId;
        [SerializeField] private GameObject root;

        [Header("Events")]
        [SerializeField] private GameEvent enteredEvent;
        [SerializeField] private GameEvent exitedEvent;
        [SerializeField] private UnityEvent entered;
        [SerializeField] private UnityEvent exited;

        public GameViewId ViewId => viewId;

        private void Reset()
        {
            root = gameObject;
        }

        public void Show()
        {
            SetVisible(true, sendEvents: true);
        }

        public void Hide()
        {
            SetVisible(false, sendEvents: true);
        }

        public void SetVisible(bool visible)
        {
            SetVisible(visible, sendEvents: true);
        }

        public void SetVisibleSilently(bool visible)
        {
            SetVisible(visible, sendEvents: false);
        }

        private void SetVisible(bool visible, bool sendEvents)
        {
            GameObject target = root != null ? root : gameObject;

            if (target.activeSelf == visible)
                return;

            target.SetActive(visible);

            if (!sendEvents)
                return;

            if (visible)
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
