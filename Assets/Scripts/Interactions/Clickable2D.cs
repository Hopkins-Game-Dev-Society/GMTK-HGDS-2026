using BirthdayJobJam.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BirthdayJobJam.Interactions
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Clickable2D : MonoBehaviour
    {
        [SerializeField] private bool ignoreClicksThroughUi = true;
        [SerializeField] private GameEvent clickedEvent;
        [SerializeField] private UnityEvent clicked;

        private void OnMouseDown()
        {
            if (ignoreClicksThroughUi
                && EventSystem.current != null
                && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Click();
        }

        public void Click()
        {
            clicked?.Invoke();
            clickedEvent?.Raise();
        }
    }
}
