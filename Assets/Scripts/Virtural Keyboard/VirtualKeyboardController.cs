using BirthdayJobJam.Core;
using BirthdayJobJam.Views;
using UnityEngine;
using UnityEngine.EventSystems;

/*
* Attached to the virtual keyboard object itself, only attached once to the overall object not to each key. 
*/


/*
namespace BirthdayJobJam.UI
{
    public sealed class VirtualKeyboardController : MonoBehaviour
    {
        public static VirtualKeyboardController Instance { get; private set; }


        [SerializeField]
        private GameplayViewStateMachine stateMachine;


        [SerializeField]
        private GameViewId keyboardView = GameViewId.Keyboard;

        public bool Shift { get; private set; }
        public bool CapsLock { get; private set; }




        private VirtualKeyboardTarget currentTarget;


        private GameViewId previousView;



        public bool HasActiveTarget =>
            currentTarget != null;



        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }


            Instance = this;


            if (stateMachine == null)
                stateMachine = Game.Ctx?.Views;
        }

        public void SetTarget(VirtualKeyboardTarget target)
        {
            if (currentTarget != null &&
                currentTarget != target)
            {
                currentTarget.Deselect();
            }


            currentTarget = target;


            if (stateMachine != null)
            {
                previousView = stateMachine.CurrentView;
            }
        }
        public void InsertSpace()
        {
            currentTarget?.Insert(" ");
        }

        public void Backspace()
        {
            currentTarget?.Backspace();
        }

        public void ToggleShift()
        {
            Shift = !Shift;
        }

        public void ToggleCapsLock()
        {
            CapsLock = !CapsLock;
        }




        public void ClearTarget()
        {
            if (currentTarget != null)
            {
                currentTarget.Deselect();
            }


            currentTarget = null;
        }

        private void OnDisable()
        {
            Debug.Log("VirtualKeyboardController disabled");
        }


        private void OnEnable()
        {
            Debug.Log("VirtualKeyboardController enabled");
        }

        public void InsertCharacter(string value)
        {
            if (currentTarget == null || string.IsNullOrEmpty(value))
                return;

            string text = value;

            // Shift XOR CapsLock determines whether letters are uppercase.
            bool uppercase = Shift ^ CapsLock;

            if (text.Length == 1 && char.IsLetter(text[0]))
            {
                text = uppercase
                    ? text.ToUpper()
                    : text.ToLower();
            }

            currentTarget.Insert(text);

            // Shift only affects the next character.
            if (Shift)
                Shift = false;
        }

        public void Finish()
        {
            if (currentTarget == null)
                return;


            ClearTarget();


            if (stateMachine != null)
                stateMachine.SwitchTo(previousView);
        }
    }
} */


namespace BirthdayJobJam.UI
{
    public sealed class VirtualKeyboardController : MonoBehaviour
    {
        public static VirtualKeyboardController Instance { get; private set; }

        [SerializeField]
        private GameplayViewStateMachine stateMachine;

        [SerializeField]
        private GameViewId keyboardView = GameViewId.Keyboard;

        [Header("Audio")]
        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private AudioClip[] keySounds = new AudioClip[3];

        public bool Shift { get; private set; }
        public bool CapsLock { get; private set; }

        private VirtualKeyboardTarget currentTarget;
        private GameViewId previousView;

        public bool HasActiveTarget => currentTarget != null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (stateMachine == null)
                stateMachine = Game.Ctx?.Views;

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        public void SetTarget(VirtualKeyboardTarget target)
        {
            if (currentTarget != null && currentTarget != target)
                currentTarget.Deselect();

            currentTarget = target;

            if (stateMachine != null)
                previousView = stateMachine.CurrentView;
        }

        public void InsertCharacter(string value)
        {
            if (currentTarget == null || string.IsNullOrEmpty(value))
                return;

            string text = value;

            bool uppercase = Shift ^ CapsLock;

            if (text.Length == 1 && char.IsLetter(text[0]))
            {
                text = uppercase
                    ? text.ToUpper()
                    : text.ToLower();
            }

            currentTarget.Insert(text);

            // Shift only affects the next character.
            if (Shift)
                Shift = false;
        }

        public void InsertSpace()
        {
            currentTarget?.Insert(" ");
        }

        public void Backspace()
        {
            currentTarget?.Backspace();
        }

        public void ToggleShift()
        {
            Shift = !Shift;
        }

        public void ToggleCapsLock()
        {
            CapsLock = !CapsLock;
        }

        public void Finish()
        {
            if (currentTarget == null)
                return;

            ClearTarget();

            if (stateMachine != null)
                stateMachine.SwitchTo(previousView);
        }

        public void ClearTarget()
        {
            if (currentTarget != null)
                currentTarget.Deselect();

            currentTarget = null;
        }

        public void PlayKeySound()
        {
            if (audioSource == null ||
                keySounds == null ||
                keySounds.Length == 0)
                return;

            AudioClip clip = keySounds[Random.Range(0, keySounds.Length)];

            if (clip != null)
                audioSource.PlayOneShot(clip);
        }

        private void OnEnable()
        {
            Debug.Log("VirtualKeyboardController enabled");
        }

        private void OnDisable()
        {
            Debug.Log("VirtualKeyboardController disabled");
        }
    }
}