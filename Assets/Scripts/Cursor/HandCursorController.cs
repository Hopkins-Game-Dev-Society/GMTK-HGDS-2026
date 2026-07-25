using UnityEngine;
using UnityEngine.UI;

//An object that follows the position of the mouse

/*
namespace BirthdayJobJam.UI
{
    public sealed class HandCursorController : MonoBehaviour
    {
        public static HandCursorController Instance { get; private set; }

        [SerializeField]
        private Sprite openHand;

        [SerializeField]
        private Sprite pointingHand;

        [SerializeField]
        private Camera worldCamera;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            Instance = this;

            spriteRenderer = GetComponent<SpriteRenderer>();

            Cursor.visible = false;
        }

        private void Update()
        {
            Vector3 position =
                worldCamera.ScreenToWorldPoint(Input.mousePosition);

            position.z = 0f;

            transform.position = position;
        }

        public void SetPointing(bool pointing)
        {
            spriteRenderer.sprite =
                pointing
                    ? pointingHand
                    : openHand;
        }
    }
} */

/*
namespace BirthdayJobJam.UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class HandCursorController : MonoBehaviour
    {
        public static HandCursorController Instance { get; private set; }

        [Header("Sprites")]
        [SerializeField]
        private Sprite openHand;

        [SerializeField]
        private Sprite pointingHand;

        [Header("References")]
        [SerializeField]
        private GameplayViewStateMachine stateMachine;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            spriteRenderer = GetComponent<SpriteRenderer>();

            Cursor.visible = false;

            if (stateMachine == null)
                stateMachine = Game.Ctx?.Views;
        }

        private void Update()
        {
            Camera activeCamera = GetActiveCamera();

            if (activeCamera == null)
                return;

            Vector3 mouse = Input.mousePosition;

            Vector3 world =
                activeCamera.ScreenToWorldPoint(mouse);

            world.z = 0f;

            transform.position = world;
        }

        private Camera GetActiveCamera()
        {
            if (stateMachine == null)
                stateMachine = Game.Ctx?.Views;

            return stateMachine != null
                ? stateMachine.CurrentCamera
                : Camera.main;
        }

        public void SetPointing(bool pointing)
        {
            spriteRenderer.sprite =
                pointing
                    ? pointingHand
                    : openHand;
        }
    }
} */



namespace BirthdayJobJam.UI
{
    [RequireComponent(typeof(Image))]
    public sealed class HandCursorController : MonoBehaviour
    {
        public static HandCursorController Instance { get; private set; }


        [Header("Cursor Sprites")]
        [SerializeField]
        private Sprite openHand;

        [SerializeField]
        private Sprite pointingHand;


        private Image image;

        private RectTransform rectTransform;



        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }


            Instance = this;


            image = GetComponent<Image>();

            rectTransform = GetComponent<RectTransform>();


            Cursor.visible = false;


            SetPointing(false);
        }



        private void Update()
        {
            FollowMouse();
        }



        private void FollowMouse()
        {
            rectTransform.position =
                Input.mousePosition;
        }



        public void SetPointing(bool pointing)
        {
            if (image == null)
                return;


            image.sprite =
                pointing
                    ? pointingHand
                    : openHand;
        }
    }
}