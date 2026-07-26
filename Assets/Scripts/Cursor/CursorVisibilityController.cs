using UnityEngine;

namespace BirthdayJobJam.UI
{
    public sealed class CursorVisibilityController : MonoBehaviour
    {
        [SerializeField] private bool cursorVisible = true;
        [SerializeField] private CursorLockMode lockState = CursorLockMode.None;

        private void Awake()
        {
            Apply();
        }

        private void OnEnable()
        {
            Apply();
        }

        public void Apply()
        {
            Cursor.lockState = lockState;
            Cursor.visible = cursorVisible;
        }
    }
}
