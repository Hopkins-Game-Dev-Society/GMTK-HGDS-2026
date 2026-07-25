using UnityEngine;

//Should be added to anything that can be clicked
//Works automatically for objects with colliders (soooo gonna need some)

namespace BirthdayJobJam.UI
{
    public sealed class CursorHover : MonoBehaviour
    {
        private void OnMouseEnter()
        {
            HandCursorController.Instance?
                .SetPointing(true);
        }


        private void OnMouseExit()
        {
            HandCursorController.Instance?
                .SetPointing(false);
        }
    }
}