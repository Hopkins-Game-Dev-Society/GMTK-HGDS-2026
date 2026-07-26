using UnityEngine;
using BirthdayJobJam.UI;

namespace BirthdayJobJam.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class DraggableObject : MonoBehaviour
    {
        [Header("Dragging")]
        [SerializeField]
        private bool returnToStartPosition = false;

        [SerializeField]
        private float followSpeed = 20f;


        private Vector3 startPosition;

        private Vector3 dragOffset;

        private bool dragging;


        private void Start()
        {
            startPosition = transform.position;
        }


        private void OnMouseDown()
        {
            dragging = true;

            Vector3 mouseWorld =
                GetMouseWorldPosition();

            dragOffset =
                transform.position - mouseWorld;
        }


        private void Update()
        {
            if (!dragging)
                return;


            Vector3 targetPosition =
                GetMouseWorldPosition()
                + dragOffset;


            transform.position =
                Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    Time.deltaTime * followSpeed
                );
        }


        private void OnMouseUp()
        {
            dragging = false;


            if (returnToStartPosition)
            {
                transform.position =
                    startPosition;
            }
        }

        private void OnMouseEnter()
        {
            HandCursorController.Instance?
                .SetPointing(true);
        }


        private void OnMouseExit()
        {
            if (!dragging)
            {
                HandCursorController.Instance?
                    .SetPointing(false);
            }
        }


        private Vector3 GetMouseWorldPosition()
        {
            Camera camera =
                BirthdayJobJam.Core.Game.Ctx.Views.CurrentCamera;


            Vector3 position =
                Input.mousePosition;


            position.z =
                Mathf.Abs(
                    camera.transform.position.z
                    - transform.position.z
                );


            return camera.ScreenToWorldPoint(position);
        }
    }
}